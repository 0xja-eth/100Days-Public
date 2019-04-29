
from django.contrib import admin

from question_module.models import QuestionType, Subject, Choice, QuestionPicture, Question
# Register your models here.
class ChoicesOfQuestion(admin.TabularInline):

    model = Choice
    extra = 4

class PicturesOfQuestion(admin.TabularInline):

    model = QuestionPicture
    extra = 1

class QuestionTypeAdmin(admin.ModelAdmin):

    list_display = ['id','text']

class SubjectAdmin(admin.ModelAdmin):

    list_display = ['id','name']

class ChoiceAdmin(admin.ModelAdmin):

    list_display = ['id','questionId','text']

class QuestionPictureAdmin(admin.ModelAdmin):

    list_display = ['id','questionId','file']

class QuestionAdmin(admin.ModelAdmin):

    list_display = ['id','title','subjectName','level','typeText','score','for_test']

    fields = ['title','subject','level','type','score','description','for_test']

    inlines = [ ChoicesOfQuestion, PicturesOfQuestion ]

admin.site.register(QuestionType, QuestionTypeAdmin)

admin.site.register(Subject, SubjectAdmin)

admin.site.register(Choice, ChoiceAdmin)

admin.site.register(QuestionPicture, QuestionPictureAdmin)

admin.site.register(Question, QuestionAdmin)