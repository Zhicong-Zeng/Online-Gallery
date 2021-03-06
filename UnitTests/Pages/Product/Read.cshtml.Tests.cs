using NUnit.Framework;
using ContosoCrafts.WebSite.Pages.Product;
using Microsoft.AspNetCore.Mvc;
using ContosoCrafts.WebSite.Models;
using System.Linq;

namespace UnitTests.Pages.Product.Read
{   
    /// <summary>
    /// Test for the read functionality of the products page
    /// </summary>
    public class ReadTests
    {
        #region TestSetup
        /// <summary>
        /// Read Model Page Model function
        /// </summary>
        public static ReadModel pageModel;

        /// <summary>
        /// Test Initializer function
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            pageModel = new ReadModel(TestHelper.ProductService);
        }
        #endregion TestSetup

        #region OnGet
        /// <summary>
        /// onGet test method to read and return the data.
        /// </summary>
        [Test]
        public void PageModel_OnGet_NotValid_Should_Return_Data()
        {
            // Arrange

            // Act
            var data = pageModel.OnGet(null);

            // Assert 
            // Return <Microsoft.AspNetCore.Mvc.RedirectToPageResult>
            Assert.IsNotNull(data);
        }

        /// <summary>
        /// onGet test method to read and return the data.
        /// </summary>
        [Test]
        public void PageModel_OnGet_Valid_Should_Return_Products()
        {
            // Arrange

            // Act
            pageModel.OnGet("the-starry-night");
            pageModel.Product.ToString();

            // Assert
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
            Assert.AreEqual("The Starry Night", pageModel.Product.Title);
        }
        #endregion OnGet

        #region GetComment
        /// <summary>
        /// get method which returns the comments
        /// </summary>
        [Test]
        public void PageModel_GetComment_Valid_Assert_Should_Return_Data()
        {
            // Arrange
            pageModel.OnGet("the-starry-night");

            // Act
            var data = pageModel.Product.Comments;

            // Assert
            Assert.IsNotNull(data);
        }
        #endregion GetComment

        #region OnPost
        /// <summary>
        /// Test OnPost method, when comments is Valid
        /// </summary>
        [Test]
        public void PageModel_Set_OnPost_Comment_Valid_Assert_Should_Return_Comments_Null()
        {
            // Arrange
            pageModel.Comment = null;
            pageModel.Product = new ProductModel
            {
                Id = "selinazawacki-moon",
                Title = "title",
                Description = "description",
                Image = "image"
            };
            // Act
            var result = pageModel.OnPost() as RedirectToPageResult; 

            // Assert 
            // nothing should be inserted
            Assert.AreEqual(null, pageModel.Product.Comments);
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
            Assert.AreEqual(true, result.PageName.Contains("Read"));
        }

        /// <summary>
        /// onPost Method when comment is blank
        /// </summary>
        [Test]
        public void PageModel_Set_OnPost_Comment_Blank_Assert_Should_Return_Comments_Null()
        {
            // Arrange
            pageModel.Comment = ""; // empty comment
            pageModel.Product = new ProductModel
            {
                Id = "selinazawacki-moon",
                Title = "title",
                Description = "description",
                Image = "image"
            };

            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;

            // Assert
            Assert.AreEqual(null, pageModel.Product.Comments); // nothing should be inserted
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
            Assert.AreEqual(true, result.PageName.Contains("Read"));
        }

        /// <summary>
        /// Test that a comment over the length limit does not get inserted
        /// but also does not block execution
        /// </summary>
        [Test]
        public void PageModel_Set_OnPost_Comment_Too_Long_Assert_Should_Return_Comments_Null()
        {
            // Arrange
            // 251 character length comment
            pageModel.Comment = "324o1324po1234u1p324u1o32i4u1po23i4" +
                "u1po23iu4p1o32iu4p1o2i3u4p1o23iu4p1oi32u4p1oi23u4p1" +
                "oi23u4p1oi3u4p1oi3u41poi3u41poi32u41poi32u41po23iu4" +
                "1poiadsfadsfadsfasdfasdfadsfadslfkja;dlskfja;ldskfj" +
                "a;ldksfj;alsdkfja;dlskfja;ldskfjdfdfdfdfdfdfdfdfdfd" +
                "fdfdfdfdfdfd";
            pageModel.Product = new ProductModel
            {
                Id = "selinazawacki-moon",
                Title = "title",
                Description = "description",
                Image = "image"
            };

            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;

            // Assert
            Assert.AreEqual(null, pageModel.Product.Comments); // nothing should be inserted
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
            Assert.AreEqual(true, result.PageName.Contains("Read"));
        }

        /// <summary>
        /// Test that a valid comment OnPost submits a comment
        /// </summary>
        [Test]
        public void PageModel_Set_OnPost_Comment_Valid_Assert_Should_Return_Comments_Valid()
        {
            // Arrange
            // 251 character length comment
            pageModel.Comment = "What a great comment this is";            
            pageModel.Product = TestHelper.ProductService.GetProducts().Last();

            // Act
            var result = pageModel.OnPost() as RedirectToPageResult;
            // Retreive the updated comment
            var data = TestHelper.ProductService.GetProducts().First(x => x.Id == pageModel.Product.Id);

            // Assert
            Assert.AreEqual(2, data.Comments.Length); // 1 comment should be added
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
            Assert.AreEqual(true, result.PageName.Contains("Read"));
        }

        /// <summary>
        /// onPost methhod to handle errors
        /// </summary>
        [Test]
        public void PageModel_OnPost_InValid_Model_NotValid_Assert_Return_False()
        {
            // Arrange

            // Force an invalid error state
            pageModel.ModelState.AddModelError("bogus", "bogus error");

            // Act
            var result = pageModel.OnPost() as ActionResult;

            // Assert
            Assert.AreEqual(false, pageModel.ModelState.IsValid);
            Assert.AreEqual(result, result);
        }
        #endregion OnPost
    }
}