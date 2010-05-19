using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.util.bamboo;
using Atlassian.plvs.util.jira;

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
                if (buildNodes.ContainsKey(getMapKeyFromBuild(build))) {
                    buildNodes[getMapKeyFromBuild(build)].Build = build;
                    if (NodesChanged != null) {
                        NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                } else {
                    buildNodes[getMapKeyFromBuild(build)] = new BuildNode(build);
                    if (NodesInserted != null) {
                        NodesInserted(this, new TreeModelEventArgs(TreePath.Empty, new[] { getIndex(build) }, new[] { getNode(build) }));
                    }
                }
                List<string> toRemove = new List<string>();
                foreach (string key in buildNodes.Keys) {
                    bool found = false;
                    foreach (BambooBuild b in builds) {
                        if (!key.Equals(getMapKeyFromBuild(b))) continue;
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

        private static string getMapKeyFromBuild(BambooBuild build) {
            return build.Server.GUID + BambooBuildUtils.getPlanKey(build);
        }

        private int getIndex(BambooBuild build) {
            int i = 0;
            foreach (string key in buildNodes.Keys) {
                if (key.Equals(getMapKeyFromBuild(build))) {
                    return i;
                }
                ++i;
            }
            throw new ArgumentException("build node not found");
        }

        private object getNode(BambooBuild build) {
            return buildNodes[getMapKeyFromBuild(build)];
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
