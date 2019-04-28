from django.db import models

# Create your models here.
class QuestionType(models.Model):
	"""
	题目类型
	"""
	text = models.CharField(max_length=4)

	def __str__(self):
		return self.text

class Subject(models.Model):
	"""
	科目
	"""
	name = models.CharField(max_length=4)	

	def __str__(self):
		return self.name


class Choice(models.Model):
	"""
	选项信息
	"""

	# 文本
	text = models.TextField()

	# 正误
	answer = models.BooleanField()

	# 所属问题ID
	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE)

	def __str__(self):
		return self.text

	def question_id(self):
		if self.question == None:
			return ''
		return self.question.id

class QuestionPicture(models.Model):
	"""
	题目图片
	"""
	file = models.ImageField(upload_to='picture/question')

	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE)

	def __str__(self):
		return self.file

	def question_id(self):
		if self.question == None:
			return ''
		return self.question.id

class Question(models.Model):
	"""
	题目信息
	"""

	# 题干
	title = models.TextField()

	# 题解
	description = models.TextField()

	# 星数
	level = models.PositiveSmallIntegerField(default=1)

	# 分值
	score = models.PositiveSmallIntegerField(default=6)

	# 科目ID
	subject = models.ForeignKey('Subject',
		default=1, blank=False, on_delete=models.CASCADE)

	# 类型ID
	type = models.ForeignKey('QuestionType',
		default=1, blank=False, on_delete=models.CASCADE)

	def __str__(self):
		return self.title

	def subject_name(self):
		if self.subject == None:
			return ''
		return self.subject.name

	def type_text(self):
		if self.type == None:
			return ''
		return self.type.text

	def convert_to_dict(self):
		return {
            'id' : self.id,
            'title' : self.title,
            'description' : self.description,
            'level' : self.level,
            'score' : self.score, 
            'subjectId' : self.subject,
            'choices' : self.choice_set.all()
        }

