using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class NewsArticleDAO
    {
        public static List<NewsArticle> GetNewsArticles()
        {
            var listNewsArticles = new List<NewsArticle>();
            try
            {
                using var context = new FunewsManagementDbContext();
                listNewsArticles = context.NewsArticles.Include(nt => nt.Tags).Include(ca => ca.Category).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listNewsArticles;
        }

        /*public static List<Tag> GetNewsArticless()
        {
            var listNewsArticles = new List<NewsArticle>();
            try
            {
                using var context = new FunewsManagementDbContext();
                listNewsArticles = context.NewsArticles.Include(c => c.Tags).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listNewsArticles;
        }*/

        public static void SaveNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.NewsArticles.Add(newsArticle);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void UpdateNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.Entry<NewsArticle>(newsArticle).State
                    = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteNewsArticle(NewsArticle newsArticle)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                var near = context.NewsArticles.SingleOrDefault(na => na.NewsArticleId == newsArticle.NewsArticleId);
                context.NewsArticles.Remove(near);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static NewsArticle GetNewsArticleById(int id)
        {
            using var context = new FunewsManagementDbContext();
            return context.NewsArticles.FirstOrDefault(na => na.NewsArticleId.Equals(id));
        }

        public static NewsArticle GetArticleWithTag(string newsArticleId)
        {
            using var context = new FunewsManagementDbContext();
            return context.NewsArticles.Include(ta => ta.Tags)
                .FirstOrDefault(ta => ta.NewsArticleId == newsArticleId);
        }



    }
}
