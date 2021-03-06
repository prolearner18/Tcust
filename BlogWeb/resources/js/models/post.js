class Post {
   constructor(data) {

      for (let property in data) {
          this[property] = data[property];
      }

	}

	static source() {
		return '/posts';
   }
	
	

	static index(params){
		let url = this.source();
		url=Helper.buildQuery(url, params);


		return new Promise((resolve, reject) => {
			axios.get(url)
				  .then(response => {
						resolve(response.data);
				  })
				  .catch(error => {
						reject(error);
				  })

		})
	}
	
	

	static details(id) {
		let url =  this.source() + `/${id}`;

		return new Promise((resolve, reject) => {
			 axios.get(url)
				  .then(response => {
						resolve(response.data);
				  })
				  .catch(error => {
						reject(error);
				  })

		})
	}

	
   static reviewedOptions() {
		return [{
			 text: '已審核',
			 value: true
		}, {
			 text: '未審核',
			 value: false
		}]
	}
	
	static topOptions() {
		return [{
			 text: '是',
			 value: true
		}, {
			 text: '否',
			 value: false
		}]
   }
   

   
}


export default Post;