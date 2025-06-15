const progressTitle = document.querySelector(".progress_title");
const progressElement = document.querySelector(".progress-bar-main");
const path = window.location.pathname;
console.log(progressElement);
///////////////////////////
async function playLessonVideo(videoUrl, lessonId) {
  const videoPlayer = document.getElementById("lessonVideo");
  videoPlayer.setAttribute("src", videoUrl);
  const res = await CheckLessonComplete(lessonId);

  if (res == false) {
    const res = await UpdateLessonProgress(lessonId);
    if (res == true) {
      handleProgress(handleGetCourseId());
    }
  }
}
const UpdateLessonProgress = async (lessonId) => {
  console.log("courseId2", lessonId);
  const courseId = handleGetCourseId();

  if (courseId && lessonId) {
    try {
      const response = await fetch(
        `http://localhost:5149/api/progress/addCompleteLesson/${courseId}/${lessonId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      const data = await response.json();
      console.log("check", data);
      return data;
    } catch (error) {
      console.error("Error fetching data:", error); // Bắt lỗi và in ra nếu có
    }
  }
  return null;
};
const CheckLessonComplete = async (lessonId) => {
  try {
    // Gọi API sử dụng fetch
    const response = await fetch(
      `http://localhost:5149/api/progress/checkLessonComplete/${lessonId}`
    );
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching data:", error); // Bắt lỗi và in ra nếu có
  }
};
//////////////////////////
const handleGetCourseId = () => {
  // Dùng biểu thức chính quy để tìm ID khóa học
  const match = path.match(/\/Course\/Details\/(\d+)/);

  if (match) {
    const courseId = match[1]; // ID khoá học
    console.log(courseId); // In ra ID khoá học, ví dụ: "1"
    return courseId;
  } else {
    console.log("Course ID not found");
  }
};
const fetchData = async (courseId) => {
  try {
    // Gọi API sử dụng fetch
    const response = await fetch(
      `http://localhost:5149/api/progress/${courseId}`
    );
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching data:", error); // Bắt lỗi và in ra nếu có
  }
};
const roundNumber = (number) => {
  // Làm tròn đến 2 chữ số sau thập phân
  const rounded = number.toFixed(2);

  // Nếu số nguyên hoặc phần thập phân là 0, trả về số nguyên
  return parseFloat(rounded) % 1 === 0
    ? parseInt(rounded)
    : parseFloat(rounded);
};
const handleProgress = async (courseid) => {
  if (courseid) {
    const data = await fetchData(courseid);
    console.log(data);

    const progress_percentage = roundNumber(
      (data?.completedLessons / data?.totalLessons) * 100
    );
    console.log(progress_percentage);

    progressTitle.innerHTML = `Đã hoàn thành ${data?.completedLessons}/${
      data?.totalLessons
    } Completed (tiến độ) ${progress_percentage ? progress_percentage : 0}%`;
    progressElement.style.width = `${progress_percentage}%`;
  }
};

handleProgress(handleGetCourseId());
/////////////////////////////
const urlvideo = document.querySelectorAll(".href_url_video");
const handleGetUrlVideoFirst = () => {
  const url = urlvideo[0].getAttribute("name");
  return url;
};
function getYouTubeId(url) {
  const regExp =
    /(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:watch\?v=|embed\/)|youtu\.be\/)([a-zA-Z0-9_-]{11})/;
  const match = url.match(regExp);
  return match ? match[1] : null; // Trả về ID hoặc null nếu không tìm thấy
}

const videoViewer = document.getElementById("lessonVideo");
handleInnerVideoLink = (link) => {
  videoViewer.src = `https://www.youtube.com/embed/${getYouTubeId(link)}`;
};
handleInnerVideoLink(handleGetUrlVideoFirst());

/////////////////////////
