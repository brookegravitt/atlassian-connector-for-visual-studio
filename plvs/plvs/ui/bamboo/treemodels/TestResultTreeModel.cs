using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo.treemodels {
    internal class TestResultTreeModel : ITreeModel {
        private readonly ICollection<BambooTest> tests;

        private readonly List<TestMethodNode> testMethods = new List<TestMethodNode>();

        public TestResultTreeModel(ICollection<BambooTest> tests, bool failedOnly) {
            this.tests = tests;
            updateModel(failedOnly);
        }

        private void updateModel(bool failedOnly) {
            testMethods.Clear();
            foreach (BambooTest test in
                tests.Where(test => !failedOnly || test.Result.Equals(BambooTest.TestResult.FAILED))) {
                testMethods.Add(new TestMethodNode(test));
            }
            if (StructureChanged != null) StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
        }

        public IEnumerable GetChildren(TreePath treePath) {
            return testMethods;
        }

        public bool IsLeaf(TreePath treePath) {
            return true;
        }

        public void updateFailedOnly(bool failedOnly) {
            updateModel(failedOnly);
        }

        public event EventHandler<TreePathEventArgs> StructureChanged;

#pragma warning disable 67
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
    }
}
