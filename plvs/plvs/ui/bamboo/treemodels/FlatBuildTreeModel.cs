using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    public class FlatBuildTreeModel : ITreeModel {

        private readonly SortedDictionary<string, BuildNode> buildNodes = new SortedDictionary<string, BuildNode>();

        public FlatBuildTreeModel(ICollection<BambooBuild> builds) {
            updateBuilds(builds);
        }

        public FlatBuildTreeModel() {
            updateBuilds(null);
        }

        public void updateBuilds(ICollection<BambooBuild> builds) {
            if (builds == null || builds.Count == 0) {
                buildNodes.Clear();
                if (StructureChanged != null) {
                    StructureChanged(this, new TreePathEventArgs(TreePath.Empty));    
                }
                return;
            }
            foreach (BambooBuild build in builds) {
                if (buildNodes.ContainsKey(build.Server.GUID + build.Key)) {
                    buildNodes[build.Server.GUID + build.Key].Build = build;
                    if (NodesChanged != null) {
                        NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                } else {
                    buildNodes[build.Server.GUID + build.Key] = new BuildNode(build);
                    if (NodesInserted != null) {
                        NodesInserted(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                }
                List<string> toRemove = new List<string>();
                foreach (string key in buildNodes.Keys) {
                    bool found = false;
                    foreach (BambooBuild b in builds) {
                        if (!key.Equals(b.Server.GUID + b.Key)) continue;
                        found = true;
                        break;
                    }
                    if (found) continue;
                    toRemove.Add(key);
                }
                foreach (string key in toRemove) {
                    BuildNode n = buildNodes[key];
                    buildNodes.Remove(key);
                    if (NodesRemoved != null) {
                        NodesRemoved(this, new TreeModelEventArgs(TreePath.Empty, new object[] {n}));
                    }
                }
            }
        }

        private int getIndex(BambooBuild build) {
            int i = 0;
            foreach (string key in buildNodes.Keys) {
                if (key.Equals(build.Server.GUID + build.Key)) {
                    return i;
                }
                ++i;
            }
            throw new ArgumentException("build node not found");
        }

        private object getNode(BambooBuild build) {
            return buildNodes[build.Server.GUID + build.Key];
        }

        public IEnumerable GetChildren(TreePath treePath) {
            return buildNodes.Values;
        }

        public bool IsLeaf(TreePath treePath) {
            return true;
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;
    }
}
