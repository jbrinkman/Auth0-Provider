namespace Dnn.Authentication.Auth0.Components
{
    /// <summary>
    /// Provides an internal structure to sort the query parameter
    /// </summary>
    public class QueryParameter
    {
        public QueryParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}