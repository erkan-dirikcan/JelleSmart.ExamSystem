Business context of this project:

This is a student placement / level assessment exam system.

Roles:
- Admin: full authority, can manage the whole system and act as Teacher or Student when needed.
- Teacher: defines course hierarchy, prepares question pools, prepares exams, assigns exams to students.
- Student: logs in, sees assigned exams, starts an active exam, answers one question per page, can move back if time remains, submits exam, gets evaluated.

Content hierarchy:
Lesson -> Class -> Unit -> Topic -> Question Pool

Question rules:
- Questions may include images.
- Answer options may include images.
- One answer is correct, others are incorrect.
- Questions are selected randomly from the topic pool.
- Answer options are shuffled for each exam attempt, so the correct answer position may vary per student.

Exam creation rules:
- Teacher sets total exam score.
- Teacher sets exam duration.
- Teacher selects topics and number of questions per topic.
- Per-question score is calculated by dividing the total exam score by total question count.

Exam solving rules:
- Student starts the exam from My Exams.
- Timer starts when the student clicks Start.
- One question per page.
- Student can navigate back while time remains.
- On finish, warn about unanswered questions.
- If time expires, exam is automatically completed.

Reporting goals:
- Student progress by topic
- weak topics
- exam-based performance
- monthly progress
- overall status