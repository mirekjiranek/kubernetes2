using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NorthwindStore.BL.DTO;
using NorthwindStore.BL.Facades.Admin.Base;
using NorthwindStore.BL.Queries;
using NorthwindStore.BL.Services;
using NorthwindStore.DAL.Entities;
using Riganti.Utils.Infrastructure.Core;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace NorthwindStore.BL.Facades.Admin
{
    public class AdminCategoriesFacade : AppCrudFacadeBase<Category, int, CategoryListDTO, CategoryDetailDTO>
    {
        private readonly ImageService imageService;

        public AdminCategoriesFacade(Func<CategoryListQuery> queryFactory, IRepository<Category, int> repository, IEntityDTOMapper<Category, CategoryDetailDTO> mapper, ImageService imageService) : base(queryFactory, repository, mapper)
        {
            this.imageService = imageService;
        }


        public Task<Stream> GetImage(int categoryId)
        {
            return imageService.GetCategoryImage(categoryId);
        }

        public Task SaveImage(int categoryId, Stream stream)
        {
            return imageService.SaveCategoryImage(categoryId, stream);
        }
    }
}
