using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Reflection;

namespace TodoAPI.Utilities
{
    public static class IQueryableExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            if (!(query is EntityQueryable<TEntity>) && !(query is InternalDbSet<TEntity>))
            {
                throw new ArgumentException("Invalid query");
            }

            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);

            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);

            var queryModel = modelGenerator.ParseQuery(query.Expression);

            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);

            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);

            var queryCompilerContext = databaseDependencies.QueryCompilationContextFactory.Create(false);

            var modelVisitor = (RelationalQueryModelVisitor)queryCompilerContext.CreateQueryModelVisitor();

            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

            var sql = modelVisitor.Queries.First().ToString();

            return sql;
        }
        
    }
}
