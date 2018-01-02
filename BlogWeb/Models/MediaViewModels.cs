﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Http;

namespace BlogWeb.Models
{
	public class MediaViewModel
	{
		public MediaViewModel() { }

		public MediaViewModel(UploadFile file)
		{
			this.id = file.Id;
			
		
			this.width = file.Width;
			this.height = file.Height;
		
			this.path = file.Path;
		}

		public int id { get; set; }

		public int postId { get; set; }

		public string itemOID { get; set; }

		public string path { get; set; }

		public string name { get; set; }

		public bool top { get; set; }

		public int width { get; set; }

		public int height { get; set; }


		
	}

	public class UploadForm
	{
		public int postId { get; set; }
		public List<IFormFile> files { get; set; }
	}
}
