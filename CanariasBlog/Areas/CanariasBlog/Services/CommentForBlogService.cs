using ClosedXML.Excel;
using CsvHelper;
using IronPdf;
using Microsoft.AspNetCore.Http;
using CanariasBlog.Areas.FiyiStack.Models;
using CanariasBlog.Areas.FiyiStack.DTOs;
using CanariasBlog.Areas.FiyiStack.Interfaces;
using CanariasBlog.Areas.CMSCore.Models;
using CanariasBlog.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;

/*
 * GUID:e6c09dfe-3a3e-461b-b3f9-734aee05fc7b
 * 
 * Coded by fiyistack.com
 * Copyright © 2023
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 */

//Last modification on: 22/02/2023 7:10:30

namespace CanariasBlog.Areas.FiyiStack.Services
{
    /// <summary>
    /// Stack:             4<br/>
    /// Name:              C# Service. <br/>
    /// Function:          Allow you to separate data contract stored in C# model from business with your clients. <br/>
    /// Also, allow dependency injection inside controllers/web apis<br/>
    /// Last modification: 22/02/2023 7:10:30
    /// </summary>
    public partial class CommentForBlogService : ICommentForBlog
    {
        private readonly IHttpContextAccessor _IHttpContextAccessor;

        public CommentForBlogService(IHttpContextAccessor IHttpContextAccessor)
        {
            _IHttpContextAccessor = IHttpContextAccessor;
        }

        #region Queries
        public CommentForBlogModel Select1ByCommentForBlogIdToModel(int CommentForBlogId)
        {
            return new CommentForBlogModel().Select1ByCommentForBlogIdToModel(CommentForBlogId);
        }

        public List<CommentForBlogModel> SelectAllToList()
        {
            return new CommentForBlogModel().SelectAllToList();
        }

        public commentforblogSelectAllPaged SelectAllPagedToModel(commentforblogSelectAllPaged commentforblogSelectAllPaged)
        {
            return new CommentForBlogModel().SelectAllPagedToModel(commentforblogSelectAllPaged);
        } 
        #endregion

        #region Non-Queries
        public int Insert(CommentForBlogModel CommentForBlogModel)
        {
            return new CommentForBlogModel().Insert(CommentForBlogModel);
        }

        public int UpdateByCommentForBlogId(CommentForBlogModel CommentForBlogModel)
        {
            return new CommentForBlogModel().UpdateByCommentForBlogId(CommentForBlogModel);
        }

        public int DeleteByCommentForBlogId(int CommentForBlogId)
        {
            return new CommentForBlogModel().DeleteByCommentForBlogId(CommentForBlogId);
        }

        public void DeleteManyOrAll(Ajax Ajax, string DeleteType)
        {
            if (DeleteType == "All")
            {
                CommentForBlogModel CommentForBlogModel = new CommentForBlogModel();
                CommentForBlogModel.DeleteAll();
            }
            else
            {
                string[] RowsChecked = Ajax.AjaxForString.Split(',');

                for (int i = 0; i < RowsChecked.Length; i++)
                {
                    CommentForBlogModel CommentForBlogModel = new CommentForBlogModel().Select1ByCommentForBlogIdToModel(Convert.ToInt32(RowsChecked[i]));
                    CommentForBlogModel.DeleteByCommentForBlogId(CommentForBlogModel.CommentForBlogId);
                }
            }
        }

        public int CopyByCommentForBlogId(int CommentForBlogId)
        {
            CommentForBlogModel CommentForBlogModel = new CommentForBlogModel().Select1ByCommentForBlogIdToModel(CommentForBlogId);
            int NewEnteredId = new CommentForBlogModel().Insert(CommentForBlogModel);

            return NewEnteredId;
        }

        public int[] CopyManyOrAll(Ajax Ajax, string CopyType)
        {
            if (CopyType == "All")
            {
                List<CommentForBlogModel> lstCommentForBlogModel = new List<CommentForBlogModel> { };
                lstCommentForBlogModel = new CommentForBlogModel().SelectAllToList();

                int[] NewEnteredIds = new int[lstCommentForBlogModel.Count];

                for (int i = 0; i < lstCommentForBlogModel.Count; i++)
                {
                    NewEnteredIds[i] = lstCommentForBlogModel[i].Insert();
                }

                return NewEnteredIds;
            }
            else
            {
                string[] RowsChecked = Ajax.AjaxForString.Split(',');
                int[] NewEnteredIds = new int[RowsChecked.Length];

                for (int i = 0; i < RowsChecked.Length; i++)
                {
                    CommentForBlogModel CommentForBlogModel = new CommentForBlogModel().Select1ByCommentForBlogIdToModel(Convert.ToInt32(RowsChecked[i]));
                    NewEnteredIds[i] = CommentForBlogModel.Insert();
                }

                return NewEnteredIds;
            }
        }

        public string PostComment(int UserId, int BlogId, string Comment)
        {
            CommentForBlogModel CommentForBlogModel = new CommentForBlogModel()
            {
                UserCreationId = UserId,
                UserLastModificationId = UserId,
                DateTimeCreation = DateTime.Now,
                DateTimeLastModification = DateTime.Now,
                Active = true,
                BlogId = BlogId,
                Comment = Comment
            };

            string NewEnteredId = CommentForBlogModel.Insert().ToString();

            UserModel UserModel = new UserModel(UserId);
            BlogModel BlogModel = new BlogModel(BlogId);

            //The system dont send email. I took this out.
            return "Message sent successfully";
        }

        public void PostLike(int BlogId)
        {

            BlogModel BlogModel = new BlogModel(BlogId);

            BlogModel.NumberOfLikes += 1;

            BlogModel.UpdateByBlogId();
        }
        #endregion

        #region Other services
        public DateTime ExportAsPDF(Ajax Ajax, string ExportationType)
        {
            var Renderer = new HtmlToPdf();
            DateTime Now = DateTime.Now;
            string RowsAsHTML = "";
            List<CommentForBlogModel> lstCommentForBlogModel = new List<CommentForBlogModel> { };

            if (ExportationType == "All")
            {
                lstCommentForBlogModel = new CommentForBlogModel().SelectAllToList();

            }
            else if (ExportationType == "JustChecked")
            {
                string[] RowsChecked = Ajax.AjaxForString.Split(',');

                foreach (string RowChecked in RowsChecked)
                {
                    CommentForBlogModel CommentForBlogModel = new CommentForBlogModel().Select1ByCommentForBlogIdToModel(Convert.ToInt32(RowChecked));
                    lstCommentForBlogModel.Add(CommentForBlogModel);
                }
            }

            foreach (CommentForBlogModel row in lstCommentForBlogModel)
            {
                RowsAsHTML += $@"{row.ToStringOnlyValuesForHTML()}";
            }

            Renderer.RenderHtmlAsPdf($@"<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""88%"" style=""width: 88% !important; min-width: 88%; max-width: 88%;"">
    <tr>
    <td align=""left"" valign=""top"">
        <font face=""'Source Sans Pro', sans-serif"" color=""#1a1a1a"" style=""font-size: 52px; line-height: 55px; font-weight: 300; letter-spacing: -1.5px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 52px; line-height: 55px; font-weight: 300; letter-spacing: -1.5px;"">Mikromatica</span>
        </font>
        <div style=""height: 25px; line-height: 25px; font-size: 23px;"">&nbsp;</div>
        <font face=""'Source Sans Pro', sans-serif"" color=""#4c4c4c"" style=""font-size: 36px; line-height: 45px; font-weight: 300; letter-spacing: -1px;"">
            <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #4c4c4c; font-size: 36px; line-height: 45px; font-weight: 300; letter-spacing: -1px;"">Registers of CommentForBlog</span>
        </font>
        <div style=""height: 35px; line-height: 35px; font-size: 33px;"">&nbsp;</div>
    </td>
    </tr>
</table>
<br>
<table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""width: 100% !important; min-width: 100%; max-width: 100%;"">
    <tr>
        <th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">CommentForBlogId&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Active&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">DateTimeCreation&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">DateTimeLastModification&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">UserCreationId&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">UserLastModificationId&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">Comment&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th><th align=""left"" valign=""top"" style=""border-width: 1px; border-style: solid; border-color: #e8e8e8; border-top: none; border-left: none; border-right: none;"">
            <font face=""'Source Sans Pro', sans-serif"" color=""#000000"" style=""font-size: 20px; line-height: 28px; font-weight: 600;"">
                <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #000000; font-size: 20px; line-height: 28px; font-weight: 600;"">BlogId&nbsp;&nbsp;&nbsp;</span>
            </font>
            <div style=""height: 10px; line-height: 10px; font-size: 8px;"">&nbsp;</div>
        </th>
    </tr>
    {RowsAsHTML}
</table>
<br>
<font face=""'Source Sans Pro', sans-serif"" color=""#868686"" style=""font-size: 17px; line-height: 20px;"">
    <span style=""font-family: 'Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #868686; font-size: 17px; line-height: 20px;"">Printed on: {Now}</span>
</font>
").SaveAs($@"wwwroot/PDFFiles/CanariasBlog/CommentForBlog/CommentForBlog_{Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}.pdf");

            return Now;
        }

        public DateTime ExportAsExcel(Ajax Ajax, string ExportationType)
        {
            DateTime Now = DateTime.Now;

            using var Book = new XLWorkbook();

            if (ExportationType == "All")
            {
                DataTable dtCommentForBlog = new DataTable();
                dtCommentForBlog.TableName = "CommentForBlog";

                //We define another DataTable dtCommentForBlogCopy to avoid issue related to DateTime conversion
                DataTable dtCommentForBlogCopy = new DataTable();
                dtCommentForBlogCopy.TableName = "CommentForBlog";

                #region Define columns for dtCommentForBlogCopy
                DataColumn dtColumnCommentForBlogIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnCommentForBlogIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnCommentForBlogIdFordtCommentForBlogCopy.ColumnName = "CommentForBlogId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnCommentForBlogIdFordtCommentForBlogCopy);

                    DataColumn dtColumnActiveFordtCommentForBlogCopy = new DataColumn();
                    dtColumnActiveFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnActiveFordtCommentForBlogCopy.ColumnName = "Active";
                    dtCommentForBlogCopy.Columns.Add(dtColumnActiveFordtCommentForBlogCopy);

                    DataColumn dtColumnDateTimeCreationFordtCommentForBlogCopy = new DataColumn();
                    dtColumnDateTimeCreationFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnDateTimeCreationFordtCommentForBlogCopy.ColumnName = "DateTimeCreation";
                    dtCommentForBlogCopy.Columns.Add(dtColumnDateTimeCreationFordtCommentForBlogCopy);

                    DataColumn dtColumnDateTimeLastModificationFordtCommentForBlogCopy = new DataColumn();
                    dtColumnDateTimeLastModificationFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnDateTimeLastModificationFordtCommentForBlogCopy.ColumnName = "DateTimeLastModification";
                    dtCommentForBlogCopy.Columns.Add(dtColumnDateTimeLastModificationFordtCommentForBlogCopy);

                    DataColumn dtColumnUserCreationIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnUserCreationIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnUserCreationIdFordtCommentForBlogCopy.ColumnName = "UserCreationId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnUserCreationIdFordtCommentForBlogCopy);

                    DataColumn dtColumnUserLastModificationIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnUserLastModificationIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnUserLastModificationIdFordtCommentForBlogCopy.ColumnName = "UserLastModificationId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnUserLastModificationIdFordtCommentForBlogCopy);

                    DataColumn dtColumnCommentFordtCommentForBlogCopy = new DataColumn();
                    dtColumnCommentFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnCommentFordtCommentForBlogCopy.ColumnName = "Comment";
                    dtCommentForBlogCopy.Columns.Add(dtColumnCommentFordtCommentForBlogCopy);

                    DataColumn dtColumnBlogIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnBlogIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnBlogIdFordtCommentForBlogCopy.ColumnName = "BlogId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnBlogIdFordtCommentForBlogCopy);

                    
                #endregion

                dtCommentForBlog = new CommentForBlogModel().SelectAllToDataTable();

                foreach (DataRow DataRow in dtCommentForBlog.Rows)
                {
                    dtCommentForBlogCopy.Rows.Add(DataRow.ItemArray);
                }

                var Sheet = Book.Worksheets.Add(dtCommentForBlogCopy);

                Sheet.ColumnsUsed().AdjustToContents();

                Book.SaveAs($@"wwwroot/ExcelFiles/CommentForBloging/CommentForBlog/CommentForBlog_{Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}.xlsx");
            }
            else if (ExportationType == "JustChecked")
            {
                string[] RowsChecked = Ajax.AjaxForString.Split(',');

                DataSet dsCommentForBlog = new DataSet();

                foreach (string RowChecked in RowsChecked)
                {
                    //We define another DataTable dtCommentForBlogCopy to avoid issue related to DateTime conversion
                    DataTable dtCommentForBlogCopy = new DataTable();
                    dtCommentForBlogCopy.TableName = "CommentForBlog";

                    #region Define columns for dtCommentForBlogCopy
                    DataColumn dtColumnCommentForBlogIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnCommentForBlogIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnCommentForBlogIdFordtCommentForBlogCopy.ColumnName = "CommentForBlogId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnCommentForBlogIdFordtCommentForBlogCopy);

                    DataColumn dtColumnActiveFordtCommentForBlogCopy = new DataColumn();
                    dtColumnActiveFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnActiveFordtCommentForBlogCopy.ColumnName = "Active";
                    dtCommentForBlogCopy.Columns.Add(dtColumnActiveFordtCommentForBlogCopy);

                    DataColumn dtColumnDateTimeCreationFordtCommentForBlogCopy = new DataColumn();
                    dtColumnDateTimeCreationFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnDateTimeCreationFordtCommentForBlogCopy.ColumnName = "DateTimeCreation";
                    dtCommentForBlogCopy.Columns.Add(dtColumnDateTimeCreationFordtCommentForBlogCopy);

                    DataColumn dtColumnDateTimeLastModificationFordtCommentForBlogCopy = new DataColumn();
                    dtColumnDateTimeLastModificationFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnDateTimeLastModificationFordtCommentForBlogCopy.ColumnName = "DateTimeLastModification";
                    dtCommentForBlogCopy.Columns.Add(dtColumnDateTimeLastModificationFordtCommentForBlogCopy);

                    DataColumn dtColumnUserCreationIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnUserCreationIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnUserCreationIdFordtCommentForBlogCopy.ColumnName = "UserCreationId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnUserCreationIdFordtCommentForBlogCopy);

                    DataColumn dtColumnUserLastModificationIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnUserLastModificationIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnUserLastModificationIdFordtCommentForBlogCopy.ColumnName = "UserLastModificationId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnUserLastModificationIdFordtCommentForBlogCopy);

                    DataColumn dtColumnCommentFordtCommentForBlogCopy = new DataColumn();
                    dtColumnCommentFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnCommentFordtCommentForBlogCopy.ColumnName = "Comment";
                    dtCommentForBlogCopy.Columns.Add(dtColumnCommentFordtCommentForBlogCopy);

                    DataColumn dtColumnBlogIdFordtCommentForBlogCopy = new DataColumn();
                    dtColumnBlogIdFordtCommentForBlogCopy.DataType = typeof(string);
                    dtColumnBlogIdFordtCommentForBlogCopy.ColumnName = "BlogId";
                    dtCommentForBlogCopy.Columns.Add(dtColumnBlogIdFordtCommentForBlogCopy);

                    
                    #endregion

                    dsCommentForBlog.Tables.Add(dtCommentForBlogCopy);

                    for (int i = 0; i < dsCommentForBlog.Tables.Count; i++)
                    {
                        dtCommentForBlogCopy = new CommentForBlogModel().Select1ByCommentForBlogIdToDataTable(Convert.ToInt32(RowChecked));

                        foreach (DataRow DataRow in dtCommentForBlogCopy.Rows)
                        {
                            dsCommentForBlog.Tables[0].Rows.Add(DataRow.ItemArray);
                        }
                    }
                    
                }

                for (int i = 0; i < dsCommentForBlog.Tables.Count; i++)
                {
                    var Sheet = Book.Worksheets.Add(dsCommentForBlog.Tables[i]);
                    Sheet.ColumnsUsed().AdjustToContents();
                }

                Book.SaveAs($@"wwwroot/ExcelFiles/CommentForBloging/CommentForBlog/CommentForBlog_{Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}.xlsx");
            }

            return Now;
        }

        public DateTime ExportAsCSV(Ajax Ajax, string ExportationType)
        {
            DateTime Now = DateTime.Now;
            List<CommentForBlogModel> lstCommentForBlogModel = new List<CommentForBlogModel> { };

            if (ExportationType == "All")
            {
                lstCommentForBlogModel = new CommentForBlogModel().SelectAllToList();

            }
            else if (ExportationType == "JustChecked")
            {
                string[] RowsChecked = Ajax.AjaxForString.Split(',');

                foreach (string RowChecked in RowsChecked)
                {
                    CommentForBlogModel CommentForBlogModel = new CommentForBlogModel().Select1ByCommentForBlogIdToModel(Convert.ToInt32(RowChecked));
                    lstCommentForBlogModel.Add(CommentForBlogModel);
                }
            }

            using (var Writer = new StreamWriter($@"wwwroot/CSVFiles/CommentForBloging/CommentForBlog/CommentForBlog_{Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}.csv"))
            using (var CsvWriter = new CsvWriter(Writer, CultureInfo.InvariantCulture))
            {
                CsvWriter.WriteRecords(lstCommentForBlogModel);
            }

            return Now;
        }
        #endregion
    }
}