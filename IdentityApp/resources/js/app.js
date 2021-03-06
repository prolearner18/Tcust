import './bootstrap';


// Vue.component('datetime-picker', require('./components/datetime-picker'));
Vue.component('page-controll', require('./components/page-controll'));
Vue.component('alert', require('./packages/components/Alert'));
Vue.component('modal', require('./packages/components/Modal'));
Vue.component('delete-confirm', require('./packages/components/DeleteConfirm'));
Vue.component('drop-down', require('./packages/components/DropDown'));
Vue.component('html-editor', require('./packages/components/HtmlEditor'));
Vue.component('check-box', require('./packages/components/Checkbox'));
Vue.component('check-box-list', require('./packages/components/CheckboxList'));



//Admin
Vue.component('client-admin', require('./views/client-admin'));
Vue.component('user-admin', require('./views/user-admin'));



// Vue.component('test', require('./views/test'));
new Vue({
    el: '#footer',
    data() {
        return {
            
            showAlert: false,
            alertSetting: {
                type: 'success',
                title: '資料儲存成功',
                text: '',
                dismissable: false,
                duration: 2500,
                class: 'fa fa-check-circle-o'
            },

            
        }
    },
    created() {
       
        Bus.$on('errors',this.showErrorMsg);
        Bus.$on('okmsg',this.showSuccessMsg);
     
    },
    methods: {
        closeAlert() {
            this.showAlert = false;
        },
        setAlertText(msg) {
            let title = msg.title ? msg.title : '處理您的要求時發生錯誤'
            let text = msg.text
            this.alertSetting.title = title
            this.alertSetting.text = text
        },
        showErrorMsg(error) {
            let msg = {}
            if (error.status == 500) {
                msg = {
                    title: '處理您的要求時發生錯誤',
                    text: '系統暫時無回應，請稍後再試'
                }
            }else if(error.status == 404){
                msg = {
                    title: '查無資料',
                    text: ''
                }
            }else {
                msg = {
                    title: error.title,
                    text: error.text
                }
            }
            this.setAlertText(msg);
            this.alertSetting.class = 'fa fa-exclamation-circle'
            this.alertSetting.type = 'danger'

            this.showAlert = true;
            this.showModal = false;
        },
        showSuccessMsg(msg) {
            this.setAlertText(msg);
            this.alertSetting.class = 'fa fa-check-circle-o'
            this.alertSetting.type = 'success'

            this.showAlert = true;
            this.showModal = false;
        },

        
        

    },
});