using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.texttag {
    class JiraIssueTagProvider {
        [Export(typeof(ITaggerProvider))]
        [ContentType("code")]
        [TagType(typeof(JiraIssueTag))]
        internal class JiraIssueTaggerProvider : ITaggerProvider {
            [Import]
            internal IClassifierAggregatorService AggregatorService;

            public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
                if (buffer == null) {
                    throw new ArgumentNullException("buffer");
                }

                return new JiraIssueTagger(AggregatorService.GetClassifier(buffer)) as ITagger<T>;
            }
        }
    }
}


