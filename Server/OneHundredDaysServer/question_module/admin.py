
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

    list_display = ['id','question_id','text']

class SubjectAdmin(admin.ModelAdmin):

    list_display = ['id','question_id','file']

class QuestionAdmin(admin.ModelAdmin):

    list_display = ['id','title','subject_name','level','type_text','score']

    fields = ['title','subject','level','type','score','description']

    inlines = [ ChoicesOfQuestion, PicturesOfQuestion ]

admin.site.register(QuestionType, QuestionTypeAdmin)

admin.site.register(Subject, SubjectAdmin)

admin.site.register(Choice)

admin.site.register(QuestionPicture)

admin.site.register(Question, QuestionAdmin)