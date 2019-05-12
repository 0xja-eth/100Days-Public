from django.db import models
from django.conf import settings
from django.utils.deconstruct import deconstructible
import os, json, time, datetime, random, base64

@deconstructible
class PictureUpload:

	def __init__(self, dir):
		self.dir = dir

	def __call__(self, instance, filename):
		# 根路径
		print("PictureUpload")
		base = os.path.join(settings.BASE_DIR, settings.STATIC_BASE)

		# 文件拓展名
		ext = '.png' # os.path.splitext(filename)[1]

		# 定义文件名,用户id_年月日时分秒_随机数
		currtime = time.strftime('%H%M%S')
		print(currtime)
		filename = "pic%d_%s_%04d" % (instance.question.id, 
			currtime, random.randint(0, 9999))

		print(filename)
		# 保存路径
		path = os.path.join(base, self.dir, filename+ext)
		print(path)

		#instance.save_changed = True

		return path

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
	answer = models.BooleanField(default=False)

	# 所属问题ID
	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE)

	def __str__(self):
		return self.text

	def questionId(self):
		if self.question == None:
			return ''
		return self.question.id

	def convertToDict(self):
		return {
			'text' : self.text,
			'answer' : self.answer
		}

class QuestionPicture(models.Model):
	"""
	题目图片
	"""
	file = models.ImageField(upload_to=PictureUpload('pictures'))

	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE)

	def __str__(self):
		return self.file.url

	def questionId(self):
		if self.question == None:
			return ''
		return self.question.id

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.file))
		if os.path.exists(path):
			return path
		else:
			raise ErrorException(ErrorType.FileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

class Question(models.Model):
	"""
	题目信息
	"""

	# 题干
	title = models.TextField()

	# 题解
	description = models.TextField(default="无")

	# 星数
	level = models.PositiveSmallIntegerField(default=1)

	# 分值
	score = models.PositiveSmallIntegerField(default=6)

	# 科目ID
	subject = models.ForeignKey('Subject', default=1, on_delete=models.CASCADE)

	# 创建时间
	create_time = models.DateTimeField(auto_now_add=True)

	# 类型ID
	type = models.ForeignKey('QuestionType', default=1, on_delete=models.CASCADE)

	for_test = models.BooleanField(default=False)

	is_deleted = models.BooleanField(default=False)

	def __str__(self):
		
		return self.title

	def subjectName(self):

		if self.subject == None:
			return ''
		return self.subject.name

	def typeText(self):

		if self.type == None:
			return ''
		return self.type.text

	def convertToDict(self):

		choices = []
		pictures = []

		choice_set = self.choice_set.all()
		for choice in choice_set:
			choices.append(choice.convertToDict())

		picture_set = self.questionpicture_set.all()
		for picture in picture_set:
			pictures.append(picture.convertToBase64())

		return {
            'id' : self.id-1,
            'title' : self.title,
            'description' : self.description,
            'level' : self.level,
            'score' : self.score, 
            'subjectId' : self.subject.id-1,
            'type' : self.type.id-1,
            'choices' : choices,
            'pictures' : pictures
        }

