using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.fields {
    public abstract class JiraFieldEditor {
        public JiraField Field { get; private set; }

        private readonly FieldValidListener validListener;
        private bool fieldValid = true;

        public static int SINGLE_LINE_EDITOR_HEIGHT = 16;
        public static int MULTI_LINE_EDITOR_HEIGHT = 80;

        public abstract Control Widget { get; }
        public abstract int VerticalSkip { get; }
        public abstract void resizeToWidth(int width);

        public delegate void FieldValidListener(JiraFieldEditor sender, bool valid);

        protected JiraFieldEditor(JiraField field, FieldValidListener validListener) {
            Field = field;
            this.validListener = validListener;
        }

        public bool FieldValid {
            get { return fieldValid; }
            protected set {
                fieldValid = value;
                if (validListener != null) {
                    validListener(this, value);
                }
            }
        }

        public virtual string getFieldLabel(JiraIssue issue, JiraField field) {
            return field.Name;
        }

        public abstract List<string> getValues();
    }
}
