namespace Zhaoxi.Manage.MentApi.Utility.FileMiddleware
{
    /// <summary>
    /// 读取图片信息
    /// </summary>
    public static class DownloadImagesExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDownloadImages(this IApplicationBuilder app, string directoryPath)
        {
            return app.UseMiddleware<DownloadImagesMiddleware>(directoryPath);
        }
    }
}
