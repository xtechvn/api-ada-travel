using ENTITIES.APPModels.ReadBankMessages;
using ENTITIES.ViewModels.MongoDb;
using System.Threading.Tasks;

namespace ServiceReceiverMedia.MongoDB
{
    public interface IImagesConvertRepository
    {
        public Task<string> InsertImage(ImagesConvertMongoDbModel item);
        public ImagesConvertMongoDbModel GetImageByURL(string url);
        public ImagesConvertMongoDbModel GetImageByURL(string url,int size);

    }
}
