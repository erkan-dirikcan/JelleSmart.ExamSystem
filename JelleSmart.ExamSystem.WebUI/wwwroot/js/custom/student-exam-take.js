"use strict";

document.addEventListener("DOMContentLoaded", function () {

    // --- DOM Elements ---
    const wrapper = document.getElementById('examQuestionsWrapper');
    const questionCards = document.querySelectorAll('.exam-question-card');
    const btnNext = document.getElementById('btnNextQuestion');
    const btnPrev = document.getElementById('btnPrevQuestion');
    const navIndicator = document.getElementById('navIndicator');
    const finishExamBtn = document.getElementById('finishExamBtn');
    const completeExamForm = document.getElementById('completeExamForm');
    
    // Header Widget Elements
    const examTimerDisplay = document.getElementById('examTimerDisplay');
    const questionCounter = document.getElementById('questionCounter');
    const scorePreview = document.getElementById('scorePreview');

    if (!wrapper) return; // Not on the exam page

    // --- State Variables ---
    let currentQuestionIndex = 0;
    const totalQuestions = parseInt(wrapper.getAttribute('data-total-questions') || 0);
    const examId = parseInt(wrapper.getAttribute('data-exam-id') || 0);
    let remainingSeconds = parseInt(wrapper.getAttribute('data-remaining-time') || 0);
    let timerInterval = null;

    // --- Initialization ---
    initTimer();
    updateNavigationUI();
    updateScorePreview();

    // --- Event Listeners ---
    
    // Navigation Buttons
    if (btnNext) {
        btnNext.addEventListener('click', () => {
            if (currentQuestionIndex < totalQuestions - 1) {
                switchQuestion(currentQuestionIndex + 1);
            }
        });
    }

    if (btnPrev) {
        btnPrev.addEventListener('click', () => {
            if (currentQuestionIndex > 0) {
                switchQuestion(currentQuestionIndex - 1);
            }
        });
    }

    // Choice Selection (Radio Buttons)
    const choiceInputs = document.querySelectorAll('.exam-choice-input');
    choiceInputs.forEach(input => {
        input.addEventListener('change', async function () {
            // UI visual update
            const parentLabel = this.closest('label.radio');
            const allLabelsInGroup = this.closest('.radio-list').querySelectorAll('label.radio');
            
            allLabelsInGroup.forEach(lbl => {
                lbl.classList.remove('border-primary', 'bg-light-primary');
                lbl.classList.add('border-light');
            });

            parentLabel.classList.remove('border-light');
            parentLabel.classList.add('border-primary', 'bg-light-primary');

            // Find question data
            const questionId = this.getAttribute('data-question-id');
            const choiceId = this.value;

            // Save via AJAX
            await saveAnswer(questionId, choiceId);
            updateScorePreview();
        });
    });

    // Finish Exam Button
    if (finishExamBtn) {
        finishExamBtn.addEventListener('click', function () {
            // Check if all questions are answered
            const answeredCount = getAnsweredQuestionsCount();
            
            if (answeredCount < totalQuestions) {
                // Show warning about unanswered questions
                Swal.fire({
                    title: "Emin misiniz?",
                    text: `Sınavı bitirmek üzeresiniz ancak ${totalQuestions - answeredCount} adet boş sorunuz var!`,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Evet, Bitir!",
                    cancelButtonText: "İptal",
                    customClass: {
                        confirmButton: "btn btn-danger",
                        cancelButton: "btn btn-light-primary"
                    }
                }).then(function(result) {
                    if (result.value) {
                        submitExam();
                    }
                });
            } else {
                // Show normal confirmation
                Swal.fire({
                    title: "Tebrikler!",
                    text: "Tüm soruları yanıtladınız. Sınavı bitirmek istiyor musunuz?",
                    icon: "success",
                    showCancelButton: true,
                    confirmButtonText: "Evet, Bitir!",
                    cancelButtonText: "İptal",
                    customClass: {
                        confirmButton: "btn btn-success",
                        cancelButton: "btn btn-light-primary"
                    }
                }).then(function(result) {
                    if (result.value) {
                        submitExam();
                    }
                });
            }
        });
    }


    // --- Core Functions ---

    function switchQuestion(newIndex) {
        // Hide current
        questionCards[currentQuestionIndex].classList.add('d-none');
        // Show new
        questionCards[newIndex].classList.remove('d-none');
        
        currentQuestionIndex = newIndex;
        updateNavigationUI();
    }

    function updateNavigationUI() {
        // Update Indicator
        if (navIndicator) {
            navIndicator.textContent = `${currentQuestionIndex + 1} / ${totalQuestions}`;
        }
        
        // Update Bottom Widget Indicator
        if (questionCounter) {
            questionCounter.textContent = `Soru ${currentQuestionIndex + 1} / ${totalQuestions}`;
        }

        // Prev Button state
        if (btnPrev) {
            if (currentQuestionIndex === 0) {
                btnPrev.classList.add('d-none');
            } else {
                btnPrev.classList.remove('d-none');
            }
        }

        // Next Button state
        if (btnNext) {
            if (currentQuestionIndex === totalQuestions - 1) {
                btnNext.classList.add('d-none');
            } else {
                btnNext.classList.remove('d-none');
            }
        }
    }

    function updateScorePreview() {
        const answeredCount = getAnsweredQuestionsCount();
        const baseScore = 100 / totalQuestions; // Dummy estimation, assuming 100 max points.
        const predictedScore = answeredCount * baseScore;
        
        if (scorePreview) {
            scorePreview.textContent = `${predictedScore.toFixed(0)} Puan (Tahmini)`;
        }
    }

    function getAnsweredQuestionsCount() {
        let count = 0;
        const processedNames = [];
        
        choiceInputs.forEach(input => {
            const name = input.name;
            if (!processedNames.includes(name)) {
                processedNames.push(name);
                const checkedRadio = document.querySelector(`input[name="${name}"]:checked`);
                if (checkedRadio) count++;
            }
        });
        return count;
    }

    async function saveAnswer(questionId, choiceId) {
        try {
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            const token = tokenElement ? tokenElement.value : '';

            const response = await fetch('/StudentExam/SubmitAnswer', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    studentExamId: examId,
                    questionId: parseInt(questionId),
                    choiceId: parseInt(choiceId)
                })
            });

            if (!response.ok) {
                console.error('Cevap kaydedilemedi:', response.status);
                // Optional: Show a subtle error toast here
            }
        } catch (error) {
            console.error('AJAX hatası:', error);
        }
    }

    function initTimer() {
        if (remainingSeconds <= 0) {
            submitExam();
            return;
        }

        renderTimerDisplay();

        timerInterval = setInterval(function () {
            remainingSeconds--;
            renderTimerDisplay();

            // Under 5 minutes warning
            if (remainingSeconds === 300) {
                const timerContainer = examTimerDisplay.closest('.bg-light');
                if (timerContainer) {
                    timerContainer.classList.remove('bg-light');
                    timerContainer.classList.add('bg-light-danger');
                    examTimerDisplay.classList.add('text-danger');
                    
                    Swal.fire({
                        title: "Süre Uyarısı",
                        text: "Sınavın bitmesine 5 dakikadan az kaldı!",
                        icon: "warning",
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 3000
                    });
                }
            }

            // Auto submit when time runs out
            if (remainingSeconds <= 0) {
                clearInterval(timerInterval);
                Swal.fire({
                    title: "Süre Doldu!",
                    text: "Sınav süreniz doldu. Sınavınız otomatik olarak sonlandırılıyor...",
                    icon: "info",
                    showConfirmButton: false,
                    allowOutsideClick: false
                });
                
                setTimeout(() => submitExam(), 2000);
            }
        }, 1000);
    }

    function renderTimerDisplay() {
        if (!examTimerDisplay) return;

        const hours = Math.floor(remainingSeconds / 3600);
        const minutes = Math.floor((remainingSeconds % 3600) / 60);
        const seconds = remainingSeconds % 60;

        examTimerDisplay.textContent = 
            `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
    }

    function submitExam() {
        if (completeExamForm) {
            completeExamForm.submit();
        }
    }

});
