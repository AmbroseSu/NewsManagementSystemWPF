using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TagService : ITagService
    {
        private readonly ITagService iTagService;

        public TagService()
        {
            iTagService = new TagService();
        }

        public void SaveTag(Tag tag)
        {
            iTagService.SaveTag(tag);
        }

        public void DeleteTag(Tag tag)
        {
            iTagService.DeleteTag(tag);
        }

        public void UpdateTag(Tag tag)
        {
            iTagService.UpdateTag(tag);
        }

        public List<Tag> GetTags()
        {
            return iTagService.GetTags();
        }
        public Tag GetTagById(int id)
        {
            return iTagService.GetTagById(id);
        }

    }
}
