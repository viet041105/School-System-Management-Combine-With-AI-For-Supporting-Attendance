# AISchool


AISchool is a comprehensive AI-powered face detection and recognition system designed to streamline attendance and access control in educational environments. This project integrates cutting-edge computer vision models with a full-stack application comprising a Python-based AI service, a .NET Core backend API, and an ASP.NET Core frontend interface.

Key Features

Face Detection & Recognition: Utilizes YOLOv11n for real-time face detection and FaceNet embeddings for accurate identity verification.

Gallery Management: Builds and maintains a gallery of known faces with pre-computed embeddings (gallery_facenetID.pkl).

RESTful API: Exposes AI functionalities via a Python FastAPI service, handling video uploads and processing.

Backend Services: Implements a .NET 8 Web API (Backendd) for business logic, user management, and integration with the AI service.

Web Frontend: Provides an ASP.NET Core (Frontendd) user interface for administrators and staff to manage users, view attendance logs, and configure settings.

Database Integration: Includes Database.sql for schema setup and CRUD operations on students, classes, and attendance records.

 How It Works 🔹 1. Register Faces Place images of each person into separate folders under Data/ImageReg/ (e.g. ImageReg/Alice, ImageReg/Bob).

Run gallery_module.py to extract face embeddings and save them to gallery_facenet1.pkl.

🔹 2. Process Video You can process a video locally with live display using:

bash Copy Edit python main.py What happens:

YOLOv11n detects faces every frame.

FaceNet compares them to the gallery.

Best face images are stored.

Final attendance is verified based on vote ratio.

🔹 3. Upload via API (Optional) Run FastAPI server:

bash Copy Edit uvicorn app:app --reload Send a request using curl or Postman:

bash Copy Edit curl -X POST "http://localhost:8000/uploadvideo" -F "video=@/path/to/video.mp4" ✅ Output Example sql Copy Edit --- Final Attendance --- Alice confirmed with vote ratio: 0.85 Bob confirmed with vote ratio: 0.73

Best face images saved to: Data/best_faces

Warning: Unknown person detected in the video.

Configuration Parameter Location Description

gallery_file gallery_module.py Path to .pkl gallery yolo_model_path main.py / app.py Path to YOLOv11n weights (.pt) match_threshold recognize_face_facenet() Similarity match for initial filtering unknown_threshold recognize_face_facenet() Threshold to classify unknown face similarity_threshold verify_attendance() Cosine similarity pass threshold vote_threshold verify_attendance() % of embeddings that must match

📸 A picture sample of our detection and recognition from camera ai :

<img width="1264" height="643" alt="Screenshot 2025-07-17 201139" src="https://github.com/user-attachments/assets/10e81c9d-6ee8-453b-8454-a8857b43f521" />


Demostration about this system : 

Web Interface Design
1 Login & Register : 
<img width="793" height="401" alt="image" src="https://github.com/user-attachments/assets/8395b43d-7266-4827-8aaa-f26f9067703f" />

<img width="785" height="397" alt="image" src="https://github.com/user-attachments/assets/72a1b5f7-a59b-4a33-9d38-8bdac59e276b" />

2.Home : 

<img width="777" height="367" alt="image" src="https://github.com/user-attachments/assets/0739d6b2-f934-47d6-9dfd-a05a795bddc0" />

3.Teacher Schedule :

<img width="709" height="312" alt="image" src="https://github.com/user-attachments/assets/b0bf6c84-4c9a-4cb3-8326-6459247de698" />

<img width="755" height="320" alt="image" src="https://github.com/user-attachments/assets/eae0dad5-976a-40e3-8d32-f2800b18b546" />

if u click button "Check with AI" it will move to this AI model to process 

<img width="593" height="632" alt="image" src="https://github.com/user-attachments/assets/44e278cd-1d2f-451c-8bee-ef9626b08ec4" />

4.Management : 

<img width="740" height="386" alt="image" src="https://github.com/user-attachments/assets/44bad601-0054-4443-ac1d-829e101cc544" />



