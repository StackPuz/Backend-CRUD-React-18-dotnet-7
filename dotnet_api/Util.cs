using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App
{
    public static class Util {
        
        public static IConfiguration configuration;
        public static string rootPath;
        public static string dateFormat = "MM/dd/yyyy";
        public static string timeFormat = "HH:mm:ss";
        public static string dateTimeFormat = "MM/dd/yyyy HH:mm:ss";

        static Util() {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        private class Filter
        {
            public string Property { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }

        private static Expression GetWhereExpression(ParameterExpression param, Filter filter)
        {
            Expression member = Expression.Property(param, filter.Property);
            if (Nullable.GetUnderlyingType(member.Type) != null) {
                member = Expression.Property(member, member.Type.GetProperty("Value"));
            }
            var isBinary = (member.Type == typeof(byte[]));
            var converter = TypeDescriptor.GetConverter(member.Type);
            object value = null;
            try {
                value = converter.ConvertFromInvariantString(filter.Value);
            }
            catch {
                if (isBinary) {
                    value = Encoding.UTF8.GetBytes(filter.Value);
                }
                else {
                    value = Activator.CreateInstance(member.Type);
                }
            }
            if (isBinary || (filter.Operator == "c" && value.GetType() != typeof(string))) {
                filter.Operator = "e";
            }
            var constant = Expression.Constant(value);
            if (filter.Operator != "c" && filter.Operator != "e") {
                if (value.GetType() == typeof(string)) {
                    var compare = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });
                    member = Expression.Call(compare, member, constant);
                    constant = Expression.Constant(0);
                }
                else if (value.GetType() == typeof(bool)) {
                    filter.Operator = "e";
                }
            }
            switch (filter.Operator)
            {
                case "c":
                    return Expression.Call(member, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), constant);
                case "e": 
                    return Expression.Equal(member, constant);
                case "g":
                    return Expression.GreaterThan(member, constant);
                case "ge":
                    return Expression.GreaterThanOrEqual(member, constant);
                case "l":
                    return Expression.LessThan(member, constant);
                case "le":
                    return Expression.LessThanOrEqual(member, constant);
                default:
                    return Expression.Equal(member, constant);
            }
        }

        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> query, string column, string oper, string value)
        {
            var filter = new Filter {
                Property = column,
                Operator = oper,
                Value = value
            };
            var param = Expression.Parameter(typeof(TEntity));
            var expression = GetWhereExpression(param, filter);
            return query.Where(Expression.Lambda<Func<TEntity, bool>>(expression, param));
        }

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string column, bool desc)
        {
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type);
            var property = type.GetProperty(column, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            var member = Expression.MakeMemberAccess(parameter, property);
            var lamda = Expression.Lambda(member, parameter);
            var method = desc ? "OrderByDescending" : "OrderBy";
            var expression = Expression.Call(typeof(Queryable), method, new Type[] { type, property.PropertyType }, query.Expression, Expression.Quote(lamda));
            return query.Provider.CreateQuery<TEntity>(expression);
        }

        public static bool IsInvalidSearch(Type type, string column) {
            if (column != null && type.GetProperty(column) == null) {
                return true;
            }
            return false;
        }

        public static void SentMail(string type, string email, string token, string user = null) {
            var body = configuration.GetValue<string>("Mail:" + type);
            body = body.Replace("{app_url}", configuration.GetValue<string>("App:URL"));
            body = body.Replace("{app_name}", configuration.GetValue<string>("App:Name"));
            body = body.Replace("{token}", token);
            if (user != null) {
                body = body.Replace("{user}", user);
            }
            var subject = (type == "Welcome" ? "Login Information" : (type == "Reset" ? "Reset Password" : configuration.GetValue<string>("App:Name") + " message"));
            /* You need to complete the SMTP Server configuration before you can sent mail
            var client = new SmtpClient(configuration.GetValue<string>("SMTP:Host"), configuration.GetValue<int>("SMTP:port"));
            client.Credentials = new NetworkCredential(configuration.GetValue<string>("SMTP:User"), configuration.GetValue<string>("SMTP:Password"));
            client.EnableSsl = true;
            client.Send(configuration.GetValue<string>("Mail:Sender"), email, subject, body);
            */
        }
        
        public static object getError(ModelStateDictionary modelState) {
            return new { errors = modelState.Where(e => e.Value.Errors.Any()).ToDictionary(e => e.Key.Replace("$.", ""), e => e.Value.Errors.First().ErrorMessage) };
        }

        public static string getFile(string path, IFormFile file) {
            if (file != null) {
                path = Path.Combine(rootPath, "uploads", path);
                var filename = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(path, filename);
                while (File.Exists(filePath)) {
                    filename = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(file.FileName);
                    filePath = Path.Combine(path, filename);
                }
                Directory.CreateDirectory(path);
                using (var target = new FileStream(filePath, FileMode.Create)) {
                    file.CopyTo(target);
                }
                return filename;
            }
            return null;
        }

        public static DateTime? GetDate(string value) {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            return DateTime.ParseExact(value, Util.dateFormat, CultureInfo.CurrentCulture);
        }

        public static TimeSpan? GetTime(string value) {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            return DateTime.ParseExact(value, Util.timeFormat, CultureInfo.CurrentCulture).TimeOfDay;
        }

        public static DateTime? GetDateTime(string value) {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }
            return DateTime.ParseExact(value, Util.dateTimeFormat, CultureInfo.CurrentCulture);
        }

        public static string formatDateStr(string value) {
            var culture = CultureInfo.InvariantCulture;
            if (value.Length == Util.timeFormat.Length) {
                return DateTime.UnixEpoch.Add(GetTime(value).Value).ToString("HH:mm:ss", culture);
            }
            else if (value.Length == Util.dateFormat.Length) {
                return GetDate(value).Value.ToString("yyyy-MM-dd", culture);
            }
            else {
                return GetDateTime(value).Value.ToString("yyyy-MM-dd HH:mm:ss", culture);
            }
        }
    }
}