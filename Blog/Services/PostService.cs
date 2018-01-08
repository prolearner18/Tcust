﻿
using Blog.Models;
using Blog.DAL;
using Blog.Specifications;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using System.Linq;
using System;

namespace Blog.Services
{
	public interface IPostService
	{
		Post Create(Post post);
		void Update(Post post, List<int> categoryIds);
		void Delete(int id, string updatedBy);
		Post GetById(int id);

		Task<IEnumerable<Post>> FetchPosts(Category category = null, string keyword = "");



		Task<IEnumerable<Post>> GetAllAsync();

		IEnumerable<Post> GetAll();

		Task<Post> GetByIdAsync(int id);

		Task<IEnumerable<Post>> GetByKeywordAsync(string keyword);

		Task<int> CheckYearAsync(int year, IEnumerable<Post> postList = null);

		//Task<IEnumerable<Post>> GetByYearAsync(int year, IEnumerable<Post> postList = null);

		//Task<IEnumerable<Post>> GetByYearMonthAsync(int year, int month);

		Task<IEnumerable<Category>> GetCategoriesAsync(bool excludeDefault = false);
		Task<Category> GetCategoryByIdAsync(int id, bool returnDefault = false);
		IList<int> GetCategoryIds(int postId);


		
	}

	public class PostService : IPostService
	{
		private readonly IBlogRepository<Post> postRepository;
		private readonly IBlogRepository<Category> categoryRepository;
		private readonly IBlogRepository<UploadFile> uploadFileRepository;

		private readonly IPostsCategoriesRepository postsCategoriesRepository;


		public PostService(IBlogRepository<Post> postRepository, IBlogRepository<Category> categoryRepository
			, IBlogRepository<UploadFile> uploadFileRepository , IPostsCategoriesRepository postsCategoriesRepository)
		{
			this.postRepository = postRepository;
			this.categoryRepository = categoryRepository;
			this.uploadFileRepository = uploadFileRepository;

			this.postsCategoriesRepository = postsCategoriesRepository;
		}

		public async Task<IEnumerable<Post>> GetAllAsync()
		{
			var filter = new BasePostFilterSpecification();
			return await postRepository.ListAsync(filter);

		}

		public async Task<IEnumerable<Post>> FetchPosts(Category category = null, string keyword = "")
		{
			Task<IEnumerable<Post>> getPostsTask;
			if (String.IsNullOrEmpty(keyword))
			{
				getPostsTask = GetAllAsync();
			}
			else
			{
				getPostsTask = GetByKeywordAsync(keyword);
			}

			var posts = await getPostsTask;

			if (category!=null)
			{
				var idsInCategory = postsCategoriesRepository.GetPostIds(category.Id);
				return posts.Where(p => idsInCategory.Contains(p.Id));

			}

			return posts;



		}

		public IEnumerable<Post> GetAll()
		{
			var filter = new PostFilterSpecification();
			return  postRepository.List(filter);
		}

		public Post GetById(int id)
		{
			var filter = new PostIdFilterSpecification(id);

			return postRepository.GetSingleBySpec(filter);

		}

		

		public async Task<Post> GetByIdAsync(int id) => await postRepository.GetByIdAsync(id);
		

		

		public async Task<IEnumerable<Post>> GetByKeywordAsync(string keyword)
		{
			var filter = new PostFilterSpecification(keyword);

			return await postRepository.ListAsync(filter);
		}

		public async Task<int> CheckYearAsync(int year, IEnumerable<Post> postList = null)
		{
			if (postList.IsNullOrEmpty()) postList = await GetAllAsync();

			var source = postList.Select(p => p.Year).Distinct();
			if (!source.Contains(year)) year = source.Max();

			return year;
		}

		//public async Task<IEnumerable<Post>> GetByYearAsync(int year, IEnumerable<Post> postList = null)
		//{
		//	var filter = new PostFilterSpecification(year);

		//	return await postRepository.ListAsync(filter);
		//}

		//public async Task<IEnumerable<Post>> GetByYearMonthAsync(int year, int month)
		//{
		//	var filter = new PostFilterSpecification(year, month);

		//	var posts = await postRepository.ListAsync(filter);

		//	return posts;
		//}

		public IEnumerable<UploadFile> GetPostAttachments(int postId)
		{
			var filter = new AttachFilterSpecification(postId);
			return uploadFileRepository.List(filter);
		}

		public Post Create(Post post)
		{
			postRepository.Add(post);
			
			return post;
		}

		public void Update(Post post, List<int> categoryIds)
		{
			postRepository.Update(post);

			postsCategoriesRepository.SyncPostCategories(post.Id, categoryIds);

		}

		public void Delete(int id, string updatedBy)
		{
			var post = postRepository.GetById(id);
			post.Removed = true;
			post.SetUpdated(updatedBy);

			postRepository.Update(post);

		}

		public async Task<IEnumerable<Category>> GetCategoriesAsync(bool excludeDefault=false)
		{
			var filter = new CategoryFilterSpecification();
			var categories= await categoryRepository.ListAsync(filter);

			if (excludeDefault) categories = categories.Where(c => c.Code != "diary").ToList();


			return categories.Where(c=>c.Active).OrderByDescending(c=>c.Order);
		}
		public async Task<Category> GetCategoryByIdAsync(int id, bool returnDefault=false)
		{
			var category = await categoryRepository.GetByIdAsync(id);
			if (category != null) return category;

			if (!returnDefault) return category;

			var  categories = await GetCategoriesAsync();
			return categories.FirstOrDefault();
		}

		public IList<int> GetCategoryIds(int postId)
		{
			return postsCategoriesRepository.GetCategoryIds(postId);
		}

		
	}
}
