using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TagDAO
    {
        public static List<Tag> GetTags()
        {
            var listTags = new List<Tag>();
            try
            {
                using var context = new FunewsManagementDbContext();
                listTags = context.Tags.ToList();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listTags;
        }

        public static void SaveTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.Tags.Add(tag);
                context.SaveChanges();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public static void UpdateTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                context.Entry<Tag>(tag).State
                    = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void DeleteTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementDbContext();
                var ta = context.Tags.SingleOrDefault(t => t.TagId == tag.TagId);
                context.Tags.Remove(ta);
                context.SaveChanges();
            }catch( Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public static Tag GetTagById(int id)
        {
            using var context = new FunewsManagementDbContext();
            return context.Tags.FirstOrDefault(ta => ta.TagId.Equals(id));
        }


    }
}
