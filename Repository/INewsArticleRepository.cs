using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface INewsArticleRepository
    {
        void SaveNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(NewsArticle newsArticle);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(int id);
        NewsArticle GetArticleWithTag(string newsArticleId);
    }
}
