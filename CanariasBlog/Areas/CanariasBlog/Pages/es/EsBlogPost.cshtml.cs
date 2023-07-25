using CanariasBlog.Areas.FiyiStack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

/*
 * GUID:e6c09dfe-3a3e-461b-b3f9-734aee05fc7b
 * Licensed to a unique person with this Token:IAmTheOwnerOfThis
 * 
 * Coded by www.fiyistack.com
 * Copyright © 2021
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 * Auto generated code. Add your custom code after the last line of auto generation
 */

//Last modification on: 16/12/2022 10:50:10

namespace CanariasBlog.Areas.FiyiStack.Pages
{
    /// <summary>
    /// Stack:             9 <br/>
    /// Name:              C# Razor Page. <br/>
    /// Function:          Allow you to show HTML files using Razor Page technology. <br/>
    /// Last modification: 16/12/2022 10:50:10
    /// </summary>
    public partial class EsBlogPostPageModel : PageModel
    {
        public void OnGet(int PostId)
        {
            BlogModel BlogModel = new BlogModel(PostId, "es");
            if (BlogModel.BlogId == 0)
            {
                // TODO This is not working but is not a big problem
                RedirectToPage("/en/Blog");
            }
            else
            {
                ViewData["PostId"] = PostId;
                ViewData["Idiom"] = "es";

                //Get UserId from Session
                int UserId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (UserId == 0)
                {
                    //User not found
                    ViewData["EnterButton"] = $@"<li class='nav-item'>
                                                <a href='/Login' class='btn btn-white mt-1 ml-2'>
                                                    <i class='fas fa-user'></i> 
                                                    <span class='nav-link-inner--text'>
                                                        Login
                                                    </span>
                                                </a>
                                            </li>";
                }
                else
                {
                    //User found
                    ViewData["EnterButton"] = $@"<li class='nav-item'>
                                                <a href='/CMSCore/DashboardIndex' class='btn btn-white mt-1 ml-2'>
                                                    <i class='fas fa-user'></i> 
                                                    <span class='nav-link-inner--text'>
                                                        Enter dashboard
                                                    </span>
                                                </a>
                                            </li>";
                }

                ViewData["og:title"] = $@"<meta property=""og:title"" content=""Blog - FiyiStack"">";
                ViewData["og:description"] = $@"<meta property=""og:description"" content=""This is the blog of FiyiStack, here I will update all data related to new generations of software in many languages"">";
                ViewData["description"] = $@"<meta name=""description"" content=""This is the blog of FiyiStack, here I will update all data related to new generations of software in many languages"">";
                ViewData["robot"] = $@"<meta name=""robots"" content=""noindex"">";
                ViewData["title"] = $@"Blog - FiyiStack";
            }
        }
    }
}