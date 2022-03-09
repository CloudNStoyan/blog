using System;

namespace Blog.Web.Areas.Admin.Users;

public class UserFilter
{
    public int Offset { get; set; }
    public int Limit { get; set; } = 20;
    public UserFilterOrderBy OrderBy { get; set; }
    public UserFilterSort Sort { get; set; }

    public static string UserFilterOrderByToSqlColumn(UserFilterOrderBy orderBy) => orderBy switch
    {
        UserFilterOrderBy.Name => "username",
        UserFilterOrderBy.Id => "user_id",
        _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Outside of ENUM values")
    };

    public static string UserFilterSortToSql(UserFilterSort sort) => sort switch
    {
        UserFilterSort.Ascending => "ASC",
        UserFilterSort.Descending => "DESC",
        _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, "Outside of ENUM values")
    };
}

public enum UserFilterOrderBy
{
    Name,
    Id
}

public enum UserFilterSort
{
    Descending,
    Ascending
}