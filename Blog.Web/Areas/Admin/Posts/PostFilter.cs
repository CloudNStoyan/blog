using System;

namespace Blog.Web.Areas.Admin.Posts
{
    public class PostFilter
    {
        public string Search { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 20;
        public PostFilterOrderBy OrderBy { get; set; }
        public PostFilterSort Sort { get; set; }
        public int UserId { get; set; }

        public static string PostFilterOrderByToSqlColumn(PostFilterOrderBy orderBy) => orderBy switch
        {
            PostFilterOrderBy.UpdatedOn => "updated_on",
            PostFilterOrderBy.CreatedOn => "created_on",
            PostFilterOrderBy.Title => "title",
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Outside of ENUM values")
        };

        public static string PostFilterSortToSql(PostFilterSort sort) => sort switch
        {
            PostFilterSort.Ascending => "ASC",
            PostFilterSort.Descending => "DESC",
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, "Outside of ENUM values")
        };
    }

    public enum PostFilterOrderBy
    {
        UpdatedOn,
        CreatedOn,
        Title
    }

    public enum PostFilterSort
    {
        Ascending,
        Descending
    }
}
