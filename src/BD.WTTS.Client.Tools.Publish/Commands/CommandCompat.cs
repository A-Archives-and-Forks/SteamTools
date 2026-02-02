namespace BD.WTTS.Client.Tools.Publish.Commands;

static partial class CommandCompat
{
    public static Option<T> GetOption<T>(string name, string? desc = null)
    {
        var o = new Option<T>(name)
        {
            Description = desc,
        };
        return o;
    }

    public static Option<T> GetOption<T>(string name, Func<T> defaultValueFactory, string? desc = null)
    {
        var o = new Option<T>(name)
        {
            Description = desc,
            DefaultValueFactory = _ => defaultValueFactory(),
        };
        return o;
    }

    public static void SetHandler(this Command command, Delegate @delegate, params Option[] options)
    {
        command.SetAction(parseResult =>
        {
            var values = options.Select(x =>
            {
                var t = x.GetType().GetGenericArguments()[0];
                var m = typeof(ParseResult).GetMethod(nameof(ParseResult.GetValue), BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(t);
                return m.Invoke(parseResult, [x]);
            }).ToArray();
            var result = @delegate.DynamicInvoke(values);
            if (result is int code)
            {
                return code;
            }
            return 0;
        });
    }
}
