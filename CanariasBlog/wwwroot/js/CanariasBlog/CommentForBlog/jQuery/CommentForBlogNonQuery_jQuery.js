

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

//Last modification on: 19/02/2023 10:32:42

//Create a formdata object
var formData = new FormData();

//Used for Quill Editor


//Used for file input


//LOAD EVENT
$(document).ready(function () {

    $("#canariasblog-commentforblog-blogid-select").on("change", function (e) {
        $("#canariasblog-commentforblog-blogid-list").html(`<li class="nav-item">
            <a class="nav-link mb-sm-3 mb-md-0 active" id="tabs-text-1-tab" data-toggle="tab" href="javascript:void(0)" role="tab" aria-controls="" aria-selected="true">
                ${$("#canariasblog-commentforblog-blogid-select option:selected").text()}
            </a>
            <input type="hidden" id="fiyistack-commentforblog-blogid-input" value="${$("#canariasblog-commentforblog-blogid-select option:selected").val()}"/>
        </li>`);
    });
    
    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.getElementsByClassName("needs-validation");
    // Loop over them and prevent submission
    Array.prototype.filter.call(forms, function (form) {
        form.addEventListener("submit", function (event) {

            event.preventDefault();
            event.stopPropagation();

            if (form.checkValidity() === true) {
                
                //CommentForBlogId
                formData.append("fiyistack-commentforblog-commentforblogid-input", $("#canariasblog-commentforblog-commentforblogid-input").val());

                formData.append("fiyistack-commentforblog-comment-input", $("#canariasblog-commentforblog-comment-input").val());
                formData.append("fiyistack-commentforblog-blogid-input", $("#canariasblog-commentforblog-blogid-input").val());
                

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
                        window.location.replace("/CanariasBlog/CommentForBlogQueryPage");
                    }
                };
                //Open connection
                xmlHttpRequest.open("POST", "/api/CanariasBlog/CommentForBlog/1/InsertOrUpdateAsync", true);
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