using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using EnvDTE;
using DteConstants = EnvDTE.Constants;

namespace Atlassian.plvs.util {
    public static class SolutionUtils {
        public static void openSolutionFile(string fileName, string lineAndColumnNumber, Solution solution) {
            List<ProjectItem> files = new List<ProjectItem>();

            matchProjectItems(fileName, files);

            if (files.Count == 0) {
                MessageBox.Show("No matching files found for " + fileName, Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else if (files.Count > 1) {
                MessageBox.Show("Multiple matching files found for " + fileName, Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } else {
                try {
                    int? lineNo = null;
                    int? columnNo = null;
                    if (lineAndColumnNumber != null) {
                        string lineNoStr = lineAndColumnNumber.Contains(",")
                                               ? lineAndColumnNumber.Substring(0, lineAndColumnNumber.IndexOf(','))
                                               : lineAndColumnNumber;
                        string columnNumberStr = lineAndColumnNumber.Contains(",")
                                                  ? lineAndColumnNumber.Substring(lineAndColumnNumber.IndexOf(',') + 1)
                                                  : null;
                        lineNo = int.Parse(lineNoStr);
                        if (columnNumberStr != null) {
                            columnNo = int.Parse(columnNumberStr);
                        }
                    }

                    Window w = files[0].Open(DteConstants.vsViewKindCode);
                    w.Visible = true;
                    w.Document.Activate();
                    TextSelection sel = w.DTE.ActiveDocument.Selection as TextSelection;
                    if (sel != null) {
                        sel.SelectAll();
                        if (lineNo.HasValue) {
                            sel.MoveToDisplayColumn(lineNo.Value, columnNo.HasValue ? columnNo.Value : 0);
//                            sel.MoveToLineAndOffset(lineNo.Value - 1, columnNo.HasValue ? columnNo.Value : 1);
//                            sel.SelectLine();
                        }
                    } else {
                        throw new Exception("Cannot get text selection for the document");
                    }
                } catch (Exception ex) {
                    PlvsUtils.showError("Unable to open the specified file", ex);
                    Debug.WriteLine(ex);
                }
            }
        }

        private static readonly List<ProjectItem> allProjectItems = new List<ProjectItem>();

        public static void refillAllSolutionProjectItems(Solution solution) {
            allProjectItems.Clear();
            foreach (Project project in solution.Projects) {
                refillProjectItems(project.ProjectItems);
            }
        }

        private static void refillProjectItems(ProjectItems items) {
            if (items == null) return;

            foreach (ProjectItem item in items) {
                allProjectItems.Add(item);
                refillProjectItems(item.ProjectItems);
            }
        }

        public static bool solutionContainsFile(string file, Solution solution) {
            List<ProjectItem> files = new List<ProjectItem>();
            matchProjectItems(file, files);
            return files.Count > 0;
        }

        private static void matchProjectItems(string file, ICollection<ProjectItem> files) {
            if (allProjectItems.Count == 0) {
                Debug.WriteLine("************ SolutionUtils.matchProjectItems() - empty project item list, have you forgotten to call refillAllSolutionProjectItems()?");
            }
            foreach (ProjectItem item in allProjectItems) {
                if (file.EndsWith(item.Name)) {
                    files.Add(item);
                }
            }
        }
    }
}
