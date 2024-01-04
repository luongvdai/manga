$(document).ready(async function () {
  toastr.options.progressBar = true;
  let params = new URLSearchParams(window.location.search);
  let CurrentPage = parseInt(params.get("page")) || 1;
  let TotalPage;
  let loginUrl = "http://localhost:5206/Home/Login";
  $("#unfollow").addClass("d-none");
  //load các loại truyện trên nav
  loadCategory();
  function loadCategory() {
    $("#categoryList").html("");
    $.ajax({
      url: "http://localhost:5278/api/Categories",
      type: "Get",
      contentType: "application/json",
      success: function (data, status, xhr) {
        $.each(data, function (index, value) {
          $("#categoryList").append($('<li class="nav-item" >'));
          appendElement = $("#categoryList li").last();
          var url = "/Home/Index/" + value["categoryId"];
          appendElement.append(
            $(
              '<a class="nav-link text-dark" aria-current="page" href="' +
                url +
                '">'
            ).html(value["categoryName"])
          );
          $("#comicCategory")
            .append(
              $("<option>")
                .attr("value", value["categoryId"])
                .html(value["categoryName"])
            )
            .last();
        });
      },
      error: (e) => {
        console.log("have some error");
      },
    });
  }

  //Thanh tìm kiếm trên navbar
  $("#searchInput").on("input", function () {
    var keyword = $(this).val();
    if (keyword != "") {
      $("#resultList").html("").removeClass("d-none");
      $.ajax({
        url: "http://localhost:5278/api/Comics/search/" + keyword,
        type: "GET",
        contentType: "application/json;utf=8",
        success: function (data, status, xhr) {
          $.each(data, function (index, value) {
            $("#resultList").append($("<li>"));
            appendElement = $("#resultList li").last();
            appendElement.html(value["comicName"]);
          });
        },
        error: (e) => {
          console.log(e);
        },
      });
    } else {
      $("#resultList").html("").addClass("d-none");
    }
  });
  // theo dõi hoặc bỏ theo dõi theo UserId và ComicId
  function handleClick(button, url, message) {
    $.ajax({
      url: url,
      type: "POST",
      contentType: "application/json",
      success: function (data, status, xhr) {
        toastr.success(message);
        $(button).addClass("d-none");
        $(button === "#follow" ? "#unfollow" : "#follow").removeClass("d-none");
      },
      error: (e) => {
        window.location.assign(loginUrl);
      },
    });
  }

  $("#follow").on("click", function () {
    handleClick(
      "#follow",
      "http://localhost:5278/api/Users/follow/1/1",
      "Theo dõi thành công!"
    );
  });
  $("#unfollow").on("click", function () {
    handleClick(
      "#unfollow",
      "http://localhost:5278/api/Users/unfollow/1/1",
      "Bỏ theo dõi thành công!"
    );
  });
  $("#changePassword").on("click", function () {
    var changePassword = {
      oldpassword: $('input[name="oldPassword"]').val(),
      newpassword: $('input[name="newPassword"]').val(),
      renewpassword: $('input[name="reNewPassword"]').val(),
    };
    $.ajax({
      url: "http://localhost:5278/api/Users/changepassword",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(changePassword),
      success: function (data, status, xhr) {
        $("#confirm")
          .removeClass("alert-danger d-none")
          .addClass("alert-succes")
          .html("Update successful!");
        $('input[name="oldPassword"]').val("");
        $('input[name="newPassword"]').val("");
        $('input[name="reNewPassword"]').val("");
      },
      error: (e) => {
        $("#confirm")
          .removeClass("alert-succes d-none")
          .addClass("alert-danger")
          .html(e.responseText);
      },
    });
  });
  $("#addComic").on("click", "button.btn-primary", function () {
    var formData = new FormData();
    formData.append("ComicName", $('input[name="comicTitle"]').val());
    formData.append("FileImage", $('input[name="comicFile"]')[0].files[0]);
    formData.append("Summary", $("#comicSummary").val());
    var categories = $("#comicCategory").val();

    $.ajax({
      url: "http://localhost:5278/api/Comics",
      type: "POST",
      processData: false,
      contentType: false,
      data: formData,
      success: function (data, status, xhr) {
        console.log(data);
        $.each(categories, function (index, value) {
          $.ajax({
            url:
              "http://localhost:5278/api/Comics/addCategory/" +
              data["comicId"] +
              "/" +
              value,
            type: "POST",
            contentType: "application/json",
            success: function (s) {},
            error: (e) => {
              $("#comicConfirm")
                .removeClass("alert-success d-none")
                .addClass("alert-danger")
                .html("Không thể thêm");
            },
          });
        });
        $("#comicConfirm")
          .removeClass("alert-danger d-none")
          .addClass("alert-success")
          .html("Thêm thành công");
        $('input[name="comicTitle"]').val("");
        $('input[name="comicFile"]')[0].files[0] = "";
        $("#comicSummary").val("");
        $("#comicCategory").val("");
      },
      error: (e) => {
        $("#comicConfirm")
          .removeClass("alert-success d-none")
          .addClass("alert-danger")
          .html("Không thể thêm");
      },
    });
  });

  $("#addNewChapter").on("show.bs.modal", function (event) {
    // Button that triggered the modal
    const button = event.relatedTarget;
    const comicId = button.getAttribute("data-bs-comicId");
    $("button#addChapter").on("click", function () {
      var chapter = {
        chapterNumber: $('input[name="chapterNumber"]').val(),
      };
      var pages = Array.from($("#chapterImage")[0].files);
      var formData = new FormData();
      for (var i = 0; i < pages.length; i++) {
        formData.append("imgPage", pages[i]);
      }
      $.ajax({
        url: "http://localhost:5278/api/Chapters/" + comicId,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(chapter),
        success: function (data, status, xhr) {
          console.log(data["chapterId"]);
          $.ajax({
            url: "http://localhost:5278/api/Pages/" + data["chapterId"],
            type: "POST",
            processData: false,
            contentType: false,
            data: formData,
            success: function () {},
            error: (e) => {
              console.log(e);
            },
          });
          $("#chapterConfirm")
            .removeClass("d-none alert-danger")
            .addClass("alert-success")
            .html("Thêm thành công!");
        },
        error: (e) => {
          if (e.status == 409) {
            $("#chapterConfirm")
              .removeClass("d-none alert-success")
              .addClass("alert-danger")
              .html("Chapter này đã tồn tại!");
          }
        },
      });
    });
  });
});
