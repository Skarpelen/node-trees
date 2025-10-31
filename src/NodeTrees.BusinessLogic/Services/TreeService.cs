using NLog;

namespace NodeTrees.BusinessLogic.Services
{
    using NodeTrees.BusinessLogic.Context;
    using NodeTrees.BusinessLogic.Models;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.Shared;

    public interface ITreeService
    {
        Task<TreeNode> GetTreeAsync(string treeName);
        Task CreateNodeAsync(string treeName, long? parentNodeId, string nodeName);
        Task DeleteNodeAsync(long nodeId);
        Task RenameNodeAsync(long nodeId, string newName);
    }

    public sealed class TreeService(IUnitOfWork unitOfWork, ICurrentUserContext context, IUserService userService) : ITreeService
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public async Task<TreeNode> GetTreeAsync(string treeName)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ValidationSecureException("treeName is required");
            }

            var ct = context.CancellationToken;
            var user = await userService.GetCurrentUserAsync(true);

            var tree = await unitOfWork.Trees.GetByNameForUserAsync(user.Id, treeName, ct);

            if (tree == null)
            {
                await unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    tree = new Tree
                    {
                        Name = treeName,
                        UserId = user.Id
                    };

                    await unitOfWork.Trees.Add(tree);
                    await unitOfWork.CompleteAsync(ct);

                    var root = new TreeNode
                    {
                        TreeId = tree.Id,
                        ParentId = null,
                        Name = treeName
                    };

                    await unitOfWork.TreeNodes.Add(root);
                    await unitOfWork.CompleteAsync(ct);
                    await unitOfWork.CommitTransactionAsync(ct);

                    _log.Info("Tree created user={UserId} name={TreeName}", user.Id, treeName);
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync(ct);
                    throw;
                }
            }

            var nodes = await unitOfWork.TreeNodes.GetByTreeIdAsync(tree.Id, ct);
            var rootNode = nodes.FirstOrDefault(n => n.ParentId == null);

            if (rootNode == null)
            {
                await unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    rootNode = new TreeNode
                    {
                        TreeId = tree.Id,
                        ParentId = null,
                        Name = tree.Name
                    };

                    await unitOfWork.TreeNodes.Add(rootNode);
                    await unitOfWork.CompleteAsync(ct);
                    await unitOfWork.CommitTransactionAsync(ct);
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync(ct);
                    throw;
                }

                nodes.Add(rootNode);
            }

            var result = BuildTreeNode(rootNode, nodes);
            return result;
        }

        public async Task CreateNodeAsync(string treeName, long? parentNodeId, string nodeName)
        {
            if (string.IsNullOrWhiteSpace(treeName))
            {
                throw new ValidationSecureException("TreeName is required");
            }

            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new ValidationSecureException("NodeName is required");
            }

            var ct = context.CancellationToken;
            var user = await userService.GetCurrentUserAsync(true);

            var tree = await unitOfWork.Trees.GetByNameForUserAsync(user.Id, treeName, ct)
                       ?? throw new NotFoundSecureException("Tree not found");

            var parent = parentNodeId.HasValue
                ? await unitOfWork.TreeNodes.GetWithTreeAsync(parentNodeId.Value, ct)
                : await unitOfWork.TreeNodes.GetRootByTreeIdAsync(tree.Id, ct);

            if (parent == null)
            {
                throw new NotFoundSecureException("Parent node not found");
            }

            if (parent.TreeId != tree.Id)
            {
                throw new ConflictSecureException("Parent node belongs to a different tree");
            }


            var exists = await unitOfWork.TreeNodes.SiblingNameExistsAsync(
                treeId: tree.Id,
                parentNodeId: parent.Id,
                nodeName: nodeName,
                ct: ct);

            if (exists)
            {
                throw new ConflictSecureException("Node with same name already exists among siblings");
            }

            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var node = new TreeNode
                {
                    TreeId = tree.Id,
                    ParentId = parent.Id,
                    Name = nodeName
                };

                await unitOfWork.TreeNodes.Add(node);
                await unitOfWork.CompleteAsync(ct);

                var parentPath = parent.Path;
                var path = string.IsNullOrWhiteSpace(parentPath)
                    ? parent.Id.ToString()
                    : $"{parentPath}/{parent.Id}";

                node.Path = path;

                await unitOfWork.TreeNodes.Update(node);
                await unitOfWork.CompleteAsync(ct);

                await unitOfWork.CommitTransactionAsync(ct);

                _log.Info("Node created treeId={TreeId} parentId={ParentId} name={Name}", tree.Id, parent.Id, nodeName);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        public async Task DeleteNodeAsync(long nodeId)
        {
            var ct = context.CancellationToken;
            var node = await unitOfWork.TreeNodes.GetWithTreeAsync(nodeId, ct);

            if (node == null)
            {
                throw new NotFoundSecureException("Node not found");
            }

            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                var root = await unitOfWork.TreeNodes.GetRootByTreeIdAsync(node.TreeId, ct);
                var isRoot = root != null && node.Id == root.Id;

                if (isRoot)
                {
                    await unitOfWork.TreeNodes.HardDeleteByTreeId(node.TreeId, ct);
                    await unitOfWork.Trees.HardDelete(node.TreeId);
                }
                else
                {
                    await unitOfWork.TreeNodes.HardDeleteSubtree(node.Id, ct);
                }

                await unitOfWork.CompleteAsync(ct);
                await unitOfWork.CommitTransactionAsync(ct);
                _log.Info("Node deleted id={Id} (root={IsRoot})", node.Id, isRoot);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        public async Task RenameNodeAsync(long nodeId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ValidationSecureException("NewName is required");
            }

            var ct = context.CancellationToken;

            var node = await unitOfWork.TreeNodes.GetWithTreeAsync(nodeId, ct)
                       ?? throw new NotFoundSecureException("Node not found");

            var isRoot = node.ParentId == null;

            if (!isRoot)
            {
                var parentId = node.ParentId!.Value;

                var exists = await unitOfWork.TreeNodes.SiblingNameExistsAsync(
                    treeId: node.TreeId,
                    parentNodeId: parentId,
                    nodeName: newName,
                    ct: ct);

                if (exists)
                {
                    throw new ConflictSecureException("Node with same name already exists among siblings");
                }
            }

            await unitOfWork.BeginTransactionAsync(ct);

            try
            {
                node.Name = newName;
                await unitOfWork.TreeNodes.Update(node);
                await unitOfWork.CompleteAsync(ct);
                await unitOfWork.CommitTransactionAsync(ct);

                _log.Info("Node renamed id={Id} newName={Name}", node.Id, newName);
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }

        private static TreeNode BuildTreeNode(TreeNode root, List<TreeNode> all)
        {
            var map = all
               .GroupBy(n => n.ParentId)
               .ToDictionary(g => g.Key ?? 0L, g => g.OrderBy(n => n.Name).ToList());

            TreeNode Build(TreeNode nodeEnt)
            {
                var node = new TreeNode
                {
                    Id = nodeEnt.Id,
                    Name = nodeEnt.Name
                };

                if (map.TryGetValue(nodeEnt.Id, out var directChildren))
                {
                    foreach (var child in directChildren)
                    {
                        var childNode = Build(child);
                        node.Children.Add(childNode);
                    }
                }

                return node;
            }

            var rootNode = Build(root);
            return rootNode;
        }
    }
}
