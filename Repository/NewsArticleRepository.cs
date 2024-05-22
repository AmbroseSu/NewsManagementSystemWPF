using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public void SaveNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.SaveNewsArticle(newsArticle);
        public void UpdateNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.UpdateNewsArticle(newsArticle);
        public void DeleteNewsArticle(NewsArticle newsArticle) => NewsArticleDAO.DeleteNewsArticle(newsArticle);
        public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.GetNewsArticles();
        public NewsArticle GetNewsArticleById(int id) => NewsArticleDAO.GetNewsArticleById(id);
        public NewsArticle GetArticleWithTag(string newsArticleId) => NewsArticleDAO.GetArticleWithTag(newsArticleId);

    }
}
