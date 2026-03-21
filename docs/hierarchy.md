# Hierarchical Unit-Topic-Question Structure

## Overview

The exam system now implements a hierarchical structure for organizing educational content. This structure allows for:
- Flexible subject-grade associations
- Ordered units within grades
- Ordered topics within units
- Questions linked to topics

## Entity Relationships

```
Subject (Ders)
    ↓ (many-to-many via SubjectGrade)
Grade (Sınıf)
    ↓ (one-to-many)
Unit (Ünite)
    ↓ (one-to-many)
Topic (Konu)
    ↓ (one-to-many)
Question (Soru)
```

### Key Changes

1. **SubjectGrade Entity**: Many-to-many relationship between Subject and Grade
   - A subject can be taught in multiple grades
   - A grade can have multiple subjects

2. **Unit Entity Changes**:
   - Added `GradeId` (required) - Units now belong to a specific grade
   - Added `Order` (int) - Units are ordered within a grade
   - Removed `SubjectId` - Subject is determined via Grade's SubjectGrades

3. **Topic Entity Changes**:
   - Added `GradeId` (required) - Topics explicitly linked to grade
   - Added `Order` (int) - Topics are ordered within a unit
   - Kept `UnitId` (required) - Topics belong to a specific unit

4. **Question Entity Simplification**:
   - Removed `GradeId` and `UnitId`
   - Only `TopicId` remains (Grade and Unit can be accessed through Topic)

## API Endpoints

### Cascading Dropdown Endpoints

#### 1. Get Grades by Subject
```
GET /Grade/BySubject/{subjectId}
```
Returns all grades associated with a specific subject.

**Response:**
```json
[
  {
    "id": 1,
    "name": "1. Sınıf",
    "order": 1
  }
]
```

#### 2. Get Units by Grade
```
GET /Unit/ByGrade/{gradeId}
```
Returns all units for a specific grade, ordered by the Order field.

**Response:**
```json
[
  {
    "id": 1,
    "name": "Sayılar",
    "gradeId": 1,
    "order": 1
  }
]
```

#### 3. Get Topics by Unit
```
GET /Topic/ByUnit/{unitId}
```
Returns all topics for a specific unit, ordered by the Order field.

**Response:**
```json
[
  {
    "id": 1,
    "name": "Doğal Sayılar",
    "unitId": 1,
    "gradeId": 1,
    "order": 1
  }
]
```

### Batch Operations

#### Get Units with Topics
```
GET /Unit/WithTopics/{gradeId}
```
Returns units with their associated topics for a specific grade.

## UI Components

### HierarchySelect.js

A JavaScript component that provides cascading dropdown selection for Subject → Grade → Unit → Topic hierarchy.

**Features:**
- Select2 integration for enhanced UI
- Automatic cascading (when parent changes, child resets)
- Loading indicators
- Error handling

**Usage:**
```javascript
const hierarchySelect = new HierarchySelect({
    subjectSelect: $('#SubjectId'),
    gradeSelect: $('#GradeId'),
    unitSelect: $('#UnitId'),
    topicSelect: $('#TopicId'),
    urls: {
        gradesBySubject: '/Grade/BySubject/',
        unitsByGrade: '/Unit/ByGrade/',
        topicsByUnit: '/Topic/ByUnit/'
    }
});

// Initialize with existing values
hierarchySelect.initialize(subjectId, gradeId, unitId, topicId);
```

## Exam Question Ordering

When creating or editing an exam, you can enable question ordering by topic sequence:

```
OrderQuestionsByTopicSequence = true
```

This will order questions within an exam based on the topic's Order field.

## Validation Rules

### Unit
- Name: Required, max length 200
- GradeId: Required
- Order: Required

### Topic
- Name: Required, max length 200
- UnitId: Required
- GradeId: Required
- Order: Required
- Unique: (GradeId, UnitId, Name) combination must be unique per grade

### Question
- QuestionText: Required
- TopicId: Required
- CorrectAnswer: Required, A, B, C, or D

## Migration Notes

### Creating the Hierarchy

1. First create Subjects and Grades
2. Create SubjectGrade associations
3. Create Units with GradeId and Order
4. Create Topics with UnitId, GradeId, and Order
5. Create Questions linked to Topics

### Sample Data Structure

```
Subject: Matematik
├── SubjectGrade → 1. Sınıf
│   ├── Unit 1: Sayılar (Order: 1)
│   │   ├── Topic 1: Doğal Sayılar (Order: 1)
│   │   └── Topic 2: Tam Sayılar (Order: 2)
│   └── Unit 2: İşlemler (Order: 2)
│       └── Topic 1: Toplama (Order: 1)
└── SubjectGrade → 2. Sınıf
    └── Unit 1: Sayılar (Order: 1)
        └── Topic 1: Doğal Sayılar (Order: 1)
```

## Files Modified

- **Entities**: `Subject.cs`, `Grade.cs`, `Unit.cs`, `Topic.cs`, `Question.cs`, `SubjectGrade.cs`
- **Repositories**: `SubjectGradeRepository.cs`, `UnitRepository.cs`, `TopicRepository.cs`
- **Controllers**: `GradeController.cs`, `UnitController.cs`, `TopicController.cs`
- **Views**: All views updated to use HierarchySelect component
- **JavaScript**: `HierarchySelect.js`, `PageUtilities.js`
- **Configuration**: `ApplicationDbContext.cs` with FluentAPI configurations

## Benefits of New Structure

1. **Clear Hierarchy**: Content is organized logically by subject → grade → unit → topic
2. **Flexibility**: Subjects can be taught across multiple grades with different content
3. **Ordering**: Units and topics can be ordered to match curriculum sequence
4. **Simplified Questions**: Questions only need to reference the topic
5. **Better Navigation**: Cascading dropdowns provide intuitive content selection
