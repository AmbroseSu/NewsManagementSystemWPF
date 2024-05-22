using BusinessObject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository iNewsArticleRepository;

        public NewsArticleService()
        {
            iNewsArticleRepository = new NewsArticleRepository();
        }

        public void SaveNewsArticle(NewsArticle newsArticle)
        {
            iNewsArticleRepository.SaveNewsArticle(newsArticle);
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            iNewsArticleRepository.UpdateNewsArticle(newsArticle);
        }
        
        public void DeleteNewsArticle(NewsArticle newsArticle)
        {
            iNewsArticleRepository.DeleteNewsArticle(newsArticle);
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return iNewsArticleRepository.GetNewsArticles();
        }

        public NewsArticle GetNewsArticleById(int id)
        {
            return iNewsArticleRepository.GetNewsArticleById(id);
        }
        public NewsArticle GetArticleWithTag(string newsArticleId)
        {
            return iNewsArticleRepository.GetArticleWithTag(newsArticleId);
        }


    }
}
