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
using System.Linq;
using DotNetNuke.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.UserDefinedTable.Controllers.Caches;
using DotNetNuke.Modules.UserDefinedTable.Models.HandlebarsTemplates;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers
{
    public class HandlebarsTemplateController : BaseDataController
    {
        #region GET

        /// <summary>
        /// Get all HandlebarsTemplates associated with module id
        /// </summary>
        /// <returns></returns>
        public HandlebarsTemplate GetTemplateByModuleId(int moduleId)
        {
            return Get<HandlebarsTemplate>(moduleId).FirstOrDefault();
        }

        /// <summary>
        /// Get a HandlebarsTemplate by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HandlebarsTemplate GetTemplate(int id)
        {
            return GetById<HandlebarsTemplate>(id);
        }

        #endregion

        #region SAVE (ADD or UPDATE)

        /// <summary>
        /// Add a new item or update the existing one
        /// </summary>
        /// <param name="template"></param>
        /// <param name="forceAdded">If true, force adding a new Template regardless checking existance</param>
        /// <returns></returns>
        public HandlebarsTemplate SaveTemplate(HandlebarsTemplate template, bool forceAdded = false)
        {
            var userInfo = UserController.GetCurrentUserInfo();
            if(userInfo == null)
                throw new Exception("Only authenticated user can perform this action");

            // delete cached html content
            DeleteCachedHtmlContentsByModuleId(template.ModuleId);
            
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<HandlebarsTemplate>();

                    var existingTemplate = GetTemplateByModuleId(template.ModuleId);

                    template.LastModifiedDate = DateTime.Now;
                    template.LastModifiedByUserId = userInfo.UserID;

                    if (existingTemplate == null)
                    {
                        template.CreatedDate = DateTime.Now;
                        template.CreatedByUserId = userInfo.UserID;
                        repository.Insert(template);
                    }
                    else
                    {
                        template.Id = existingTemplate.Id;
                        template.CreatedByUserId = existingTemplate.CreatedByUserId;
                        template.CreatedDate = existingTemplate.CreatedDate;
                        repository.Update(template);
                    }

                    return template;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof(HandlebarsTemplate), ex.Message, ex.StackTrace);
                return null;
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Delete a HandlebarsTemplate by id
        /// </summary>
        /// <param name="templateId"></param>
        public bool DeleteTemplate(int templateId)
        {
            var template = GetTemplate(templateId);
            return template != null && DeleteTemplate(template);
        }

        /// <summary>
        /// Delete a HandlebarsTemplate
        /// </summary>
        /// <param name="template"></param>
        public bool DeleteTemplate(HandlebarsTemplate template)
        {
            // delete cached html content
            DeleteCachedHtmlContentsByModuleId(template.ModuleId);
            
            return Delete(template);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Delete cached html contents
        /// </summary>
        /// <param name="moduleId"></param>
        private static void DeleteCachedHtmlContentsByModuleId(int moduleId)
        {
            new CachedHtmlContentController().DeleteCachedHtmlContentsByModuleId(moduleId);
        }

        #endregion
    }
}
