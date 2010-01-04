using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.models.fields;

namespace Atlassian.plvs.models {
    class JiraActionFieldType {

        public enum WidgetType {
            UNSUPPORTED,
            ASSIGNEE,
            REPORTER,
            ISSUE_TYPE,
            SUMMARY,
            DESCRIPTION,
            ENVIRONMENT,
            DUE_DATE,
            TIMETRACKING,
            VERSIONS,
            FIX_VERSIONS,
            RESOLUTION,
            PRIORITY,
            COMPONENTS,
            SECURITY
        }
 
        public sealed class WidgetTypeAndFieldFiller {
		    public int SortOrder { get; private set; }
		    public FieldFiller Filler { get; private set; }
		    public WidgetType WidgetType { get; private set; }

            private static int sortOrderCounter;

		    public WidgetTypeAndFieldFiller(WidgetType widgetType, FieldFiller filler) {
			    SortOrder = sortOrderCounter++;
			    Filler = filler;
			    WidgetType = widgetType;
		    }
	    }

        private static readonly SortedDictionary<string, WidgetTypeAndFieldFiller> TypeMap = 
            new SortedDictionary<string, WidgetTypeAndFieldFiller> {
		    
            { "summary", new WidgetTypeAndFieldFiller(WidgetType.SUMMARY, new SummaryFiller()) },
		    { "resolution", new WidgetTypeAndFieldFiller(WidgetType.RESOLUTION, new ResolutionFiller()) },
		    { "issuetype", new WidgetTypeAndFieldFiller(WidgetType.ISSUE_TYPE, new IssueTypeFiller()) },
		    { "priority", new WidgetTypeAndFieldFiller(WidgetType.PRIORITY, new PriorityFiller()) },
		    { "duedate", new WidgetTypeAndFieldFiller(WidgetType.DUE_DATE, new DueDateFiller()) },
		    { "components", new WidgetTypeAndFieldFiller(WidgetType.COMPONENTS, new ComponentsFiller()) },
		    { "versions", new WidgetTypeAndFieldFiller(WidgetType.VERSIONS, new AffectsVersionsFiller()) },
		    { "fixVersions", new WidgetTypeAndFieldFiller(WidgetType.FIX_VERSIONS, new FixVersionsFiller()) },
		    { "assignee", new WidgetTypeAndFieldFiller(WidgetType.ASSIGNEE, new AssigneeFiller()) },
		    { "reporter", new WidgetTypeAndFieldFiller(WidgetType.REPORTER, new ReporterFiller()) },
		    { "environment", new WidgetTypeAndFieldFiller(WidgetType.ENVIRONMENT, new EnvironmentFiller()) },
		    { "description", new WidgetTypeAndFieldFiller(WidgetType.DESCRIPTION, new DescriptionFiller()) },
		    { "timetracking", new WidgetTypeAndFieldFiller(WidgetType.TIMETRACKING, new TimeTrackingFiller()) },
		    { "security", new WidgetTypeAndFieldFiller(WidgetType.SECURITY, new SecurityFiller()) }
        };

	    private static readonly CustomFieldFiller CustomFieldFiller = new CustomFieldFiller(); 

       	private JiraActionFieldType() {}

	    public static WidgetType getFieldTypeForFieldId(JiraField field) {
		    return getFieldTypeForFieldId(field.Id);
	    }

	    public static WidgetType getFieldTypeForFieldId(string fieldId) {
	        return TypeMap.ContainsKey(fieldId) ? TypeMap[fieldId].WidgetType : WidgetType.UNSUPPORTED;
	    }

	    public static List<JiraField> fillFieldValues(JiraIssue issue, List<JiraField> fields) {
		    List<JiraField> result = new List<JiraField>();

		    foreach (JiraField field in fields) {
			    JiraField filledField = fillField(issue, field);
			    if (filledField != null) {
				    result.Add(filledField);
			    }
		    }

		    addTimeFields(issue, result);

		    return result;
	    }

	    private static void addTimeFields(JiraIssue issue, ICollection<JiraField> result) {
		    string originalEstimate = issue.OriginalEstimateInSeconds.ToString();
		    string remainingEstimate = issue.RemainingEstimateInSeconds.ToString();
		    string timeSpent = issue.TimeSpentInSeconds.ToString();

		    if (originalEstimate != null) {
			    JiraField originalEstimateField = new JiraField("timeoriginalestimate", "Original Estimate");
			    originalEstimateField.Values.Add(originalEstimate);
			    result.Add(originalEstimateField);
		    }
		    if (remainingEstimate != null) {
			    JiraField remainingEstimateField = new JiraField("timeestimate", "Remaining Estimate");
			    remainingEstimateField.Values.Add(remainingEstimate);
			    result.Add(remainingEstimateField);
		    }
		    if (timeSpent != null) {
			    JiraField timeSpentField = new JiraField("timespent", "Time Spent");
			    timeSpentField.Values.Add(timeSpent);
			    result.Add(timeSpentField);
		    }
	    }

	    private static JiraField fillField(JiraIssue issue, JiraField field) {
            JiraField result = new JiraField(field);
            if (TypeMap.ContainsKey(field.Id)) {
                WidgetTypeAndFieldFiller widgetTypeAndFieldFiller = TypeMap[field.Id];
                result.Values = widgetTypeAndFieldFiller.Filler.getFieldValues(field.Id, issue);
		    } else {
			    result.Values = CustomFieldFiller.getFieldValues(field.Id, issue);
		    }
		    return result;
	    }

	    public static ICollection<JiraField> sortFieldList(List<JiraField> fieldList) {
		    int customFieldOffset = TypeMap.Count;
		    SortedDictionary<int, JiraField> sorted = new SortedDictionary<int, JiraField>();
		    foreach (JiraField field in fieldList) {
			    if (TypeMap.ContainsKey(field.Id)) {
				    sorted[TypeMap[field.Id].SortOrder] = field;
			    } else {
				    sorted[customFieldOffset++] = field;
			    }
		    }

		    return sorted.Values;
	    } 
    }
}
