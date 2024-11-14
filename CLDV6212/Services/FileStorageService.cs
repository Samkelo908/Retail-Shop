using Azure.Storage.Files.Shares;

namespace CLDV6212.Services
{
    public class FileStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;

        public FileStorageService(ShareServiceClient shareServiceClient)
        {
            _shareServiceClient = shareServiceClient;
        }

        public async Task UploadFileAsync(string shareName, string directoryName, string fileName, Stream fileStream)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            var directoryClient = shareClient.GetDirectoryClient(directoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(fileStream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, fileStream.Length), fileStream);
        }

        public async Task<Stream> DownloadFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            var directoryClient = shareClient.GetDirectoryClient(directoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            var downloadResponse = await fileClient.DownloadAsync();
            return downloadResponse.Value.Content;
        }

        public async Task DeleteFileAsync(string shareName, string directoryName, string fileName)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            var directoryClient = shareClient.GetDirectoryClient(directoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
    }
}