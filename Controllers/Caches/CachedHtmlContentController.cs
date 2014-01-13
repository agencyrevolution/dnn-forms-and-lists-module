/*
' Copyright (c) 2013 Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Data;
using DotNetNuke.Modules.UserDefinedTable.Models.Caches;
using log4net;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers.Caches
{
    public class CachedHtmlContentController : BaseDataController
    {
        protected new ILog Logger = LogManager.GetLogger(typeof (CachedHtmlContentController));

        #region GET

        /// <summary>
        /// Get CachedHtmlContent entity
        /// </summary>
        /// <returns></returns>
        public CachedHtmlContent GetCachedHtmlContent(int moduleId, int skip, int limit, bool isEditMode, string keyword = null)
        {
            var conditions = new Dictionary<string, object>
            {
                {CachedHtmlContent.ModuleIdProperty, moduleId},
                {CachedHtmlContent.SkipProperty, skip},
                {CachedHtmlContent.LimitProperty, limit},
                {CachedHtmlContent.IsEditModeProperty, isEditMode? 1: 0},
                {CachedHtmlContent.KeywordProperty, keyword}
            };

            return Get<CachedHtmlContent>(
                conditions).FirstOrDefault();
        }

        #endregion

        #region Save

        /// <summary>
        /// Add a new item or update the existing one
        /// </summary>
        /// <param name="cachedHtmlContent"></param>
        /// <returns></returns>
        public CachedHtmlContent SaveCachedHtmlContent(CachedHtmlContent cachedHtmlContent)
        {
            if (cachedHtmlContent == null)
                throw new ArgumentNullException("cachedHtmlContent", @"Parameter must not be null");

            using (var context = DataContext.Instance())
            {
                var repository = context.GetRepository<CachedHtmlContent>();
                var existingItem = GetCachedHtmlContent(cachedHtmlContent.ModuleId, cachedHtmlContent.Skip,
                    cachedHtmlContent.Limit, cachedHtmlContent.IsEditMode, cachedHtmlContent.Keyword);
                if (existingItem == null)
                    repository.Insert(cachedHtmlContent);
                else
                    repository.Update(cachedHtmlContent);

                return cachedHtmlContent;
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete a CachedHtmlContent by module id
        /// </summary>
        /// <param name="moduleId"></param>
        public bool DeleteCachedHtmlContentsByModuleId(int moduleId)
        {
            return Delete<CachedHtmlContent>(CachedHtmlContent.ModuleIdProperty, moduleId);
        }

        /// <summary>
        /// Delete a CachedHtmlContent entity
        /// </summary>
        /// <param name="cachedHtmlContent"></param>
        public bool DeleteCachedHtmlContent(CachedHtmlContent cachedHtmlContent)
        {
            return Delete(cachedHtmlContent);
        }

        #endregion
    }
}
