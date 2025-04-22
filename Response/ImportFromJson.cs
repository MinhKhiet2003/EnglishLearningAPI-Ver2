using System.Collections.Generic;

namespace EnglishLearningAPI.Response
{
    public class ImportFromJson
    {
        public int CountSuccess { get; set; }
        public int CountError { get; set; }
        public List<string> Error { get; set; }

        // Constructor không tham số
        public ImportFromJson()
        {
        }

        // Constructor có tham số
        public ImportFromJson(int countSuccess, int countError, List<string> error = null)
        {
            CountSuccess = countSuccess;
            CountError = countError;
            Error = error;
        }
    }
}
