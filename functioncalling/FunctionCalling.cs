
using Azure.AI.OpenAI;
using System.Text.Encodings.Web;
using System.Text.Json;

//using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using System.Reflection;
using System.ComponentModel;
using System.Text;

public class FunctionCalling  {


    public FunctionDefinition[] GetDefinitionsFromObject(Object o) {
        // GET ALL PUBLIC METHODS
        MethodInfo[] methodInfos = o.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) ;
        FunctionDefinition[] fds = new FunctionDefinition[methodInfos.Length];
        for (int i = 0; i < methodInfos.Length; i++) {
            MethodInfo mi = methodInfos[i];

            fds[i] = new FunctionDefinition();
            fds[i].Name = mi.Name;
            // Get description from attribute
            fds[i].Description = mi.GetCustomAttribute<DescriptionAttribute>()?.Description;
            // Get parameters
            ParameterInfo[] parameters = mi.GetParameters();
            StringBuilder stringBuilder = new StringBuilder("{ \"type\": \"object\", \"properties\": {");
            for (int j = 0; j < parameters.Length; j++) {
                ParameterInfo pi = parameters[j];
                stringBuilder.Append("\"").Append(pi.Name).Append("\"");
                stringBuilder.Append(":");
                stringBuilder.Append("{").Append("\"Type\":\"").Append(MapType(pi.ParameterType)).Append("\"");
                var description = pi.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (description != null) {
                    stringBuilder.Append(",\"Description\":\"").Append(description).Append("\"");
                }
                stringBuilder.Append("}");
                if (j < parameters.Length - 1) {
                    stringBuilder.Append(", ");
                }
                
            }
            stringBuilder.Append("}}");
            fds[i].Parameters = BinaryData.FromString(stringBuilder.ToString());
        }
        return fds;
    }

    private string MapType(Type parameterType)
    {
        if (parameterType == typeof(string)) {
            return "string";
        }
        if (parameterType == typeof(int)) {
            return "integer";
        }
        if (parameterType == typeof(bool)) {
            return "boolean";
        }
        if (parameterType == typeof(decimal)) {
            return "number";
        }
        if (parameterType == typeof(double)) {
            return "number";
        }
        throw new Exception("Type " + parameterType.Name + " not supported!");
    }

    public string ExecuteFunction(FunctionCall call, Object o) {
        Console.WriteLine("ExecuteFunction " + call.Name + " called");
        MethodInfo? mi = o?.GetType()?.GetMethod(call.Name);
        if (mi == null) {
            return "\"Function " + call.Name + " not found!\"";
        }
        ParameterInfo[] parameters = mi.GetParameters();
        Object?[] args = new Object[parameters.Length];
        var arguments = call.Arguments;
        Console.WriteLine("Arguments: " + arguments);
        var completeObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(arguments);
        
        for (int i = 0; i < parameters.Length; i++) {
            ParameterInfo pi = parameters[i];
            Console.WriteLine("Parameter " + pi.Name + " of type " + pi.ParameterType.Name);
            JsonElement? element = completeObject?.GetValueOrDefault(pi.Name);
            if (element != null) {
                Console.WriteLine("Element: " + element);
                if (pi.ParameterType == typeof(string)) {
                    args[i] = element?.ToString();
                }
                if (pi.ParameterType == typeof(int)) {
                    args[i] = element?.GetInt32();
                }
                if (pi.ParameterType == typeof(bool)) {
                    args[i] = element?.GetBoolean();
                }
                if (pi.ParameterType == typeof(decimal)) {
                    args[i] = element?.GetDecimal();
                }
                if (pi.ParameterType == typeof(double)) {
                    args[i] = element?.GetDouble();
                }
            }
        }
        Object? result = mi.Invoke(o, args);
        return JsonSerializer.Serialize(result);
    }

    private object GetProperty(dynamic d, string name) {
        return d.GetType()
            .GetProperty(name)
                .GetValue(d, null);
    }

}