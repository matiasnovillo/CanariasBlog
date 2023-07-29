

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

//Stack: 10

//Last modification on: 24/03/2023 17:29:08

//Create a formdata object
var formData = new FormData();

//Used for Quill Editor
let fiyistackblogbodytoolbaroptions = [
    ["bold", "italic", "underline", "strike"],        // toggled buttons
    ["link", "blockquote", "code-block"],

    [{ "header": 1 }, { "header": 2 }],               // custom button values
    [{ "list": "ordered" }, { "list": "bullet" }],
    [{ "script": "sub" }, { "script": "super" }],      // superscript/subscript
    [{ "indent": "-1" }, { "indent": "+1" }],          // outdent/indent
    [{ "direction": "rtl" }],                         // text direction
    ["image", "video"],
    ["clean"]                                         // remove formatting button
];
let fiyistackblogbodyquill = new Quill("#canariasblog-blog-body-input", {
    modules: {
        toolbar: fiyistackblogbodytoolbaroptions
    },
    theme: "snow"
});


//Used for file input
let fiyistackblogbackgroundimageinput;
let fiyistackblogbackgroundimageboolfileadded;
$("#canariasblog-blog-backgroundimage-input").on("change", function (e) {
    fiyistackblogbackgroundimageinput = $(this).get(0).files;
    fiyistackblogbackgroundimageboolfileadded = true;
    formData.append("canariasblog-blog-backgroundimage-input", fiyistackblogbackgroundimageinput[0], fiyistackblogbackgroundimageinput[0].name);
});



//LOAD EVENT
$(document).ready(function () {
    fiyistackblogbodyquill.root.innerHTML = $("#canariasblog-blog-body-hidden-value").val();
    
    
    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.getElementsByClassName("needs-validation");
    // Loop over them and prevent submission
    Array.prototype.filter.call(forms, function (form) {
        form.addEventListener("submit", function (event) {

            event.preventDefault();
            event.stopPropagation();

            if (form.checkValidity() === true) {
                
                //BlogId
                formData.append("canariasblog-blog-blogid-input", $("#canariasblog-blog-blogid-input").val());

                formData.append("canariasblog-blog-title-input", $("#canariasblog-blog-title-input").val());
                formData.append("canariasblog-blog-body-input", fiyistackblogbodyquill.root.innerHTML);
                if (!fiyistackblogbackgroundimageboolfileadded) {
                    formData.append("canariasblog-blog-backgroundimage-input", $("#canariasblog-blog-backgroundimage-readonly").val());
                }
                formData.append("canariasblog-blog-numberoflikes-input", $("#canariasblog-blog-numberoflikes-input").val());
                formData.append("canariasblog-blog-numberofcomments-input", $("#canariasblog-blog-numberofcomments-input").val());
                

                //Setup request
                var xmlHttpRequest = new XMLHttpRequest();
                //Set event listeners
                xmlHttpRequest.upload.addEventListener("loadstart", function (e) {
                    //SAVING
                    $.notify({ message: "Saving data. Please, wait" }, { type: "info", placement: { from: "bottom", align: "center" } });
                });
                xmlHttpRequest.onload = function () {
                    if (xmlHttpRequest.status >= 400) {
                        //ERROR
                        console.log(xmlHttpRequest);
                        $.notify({ icon: "fas fa-exclamation-triangle", message: "There was an error while saving the data" }, { type: "danger", placement: { from: "bottom", align: "center" } });
                    }
                    else {
                        //SUCCESS
                        window.location.replace("/CanariasBlog/BlogQueryPage");
                    }
                };
                //Open connection
                xmlHttpRequest.open("POST", "/api/CanariasBlog/Blog/1/InsertOrUpdateAsync", true);
                //Send request
                xmlHttpRequest.send(formData);
            }
            else {
                $.notify({ message: "Please, complete all fields." }, { type: "warning", placement: { from: "bottom", align: "center" } });
            }


            form.classList.add("was-validated");
        }, false);
    });
});