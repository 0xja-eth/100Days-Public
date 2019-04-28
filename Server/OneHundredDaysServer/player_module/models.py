from django.db import models
from django.conf import settings
import os, base64, json

# Create your models here.
@deconstructible
class SavefileUpload:

	def __init__(self, dir):
		self.dir = dir

	def __call__(self, instance, filename):
		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名,用户id_年月日时分秒_随机数
		currtime = time.strftime('%Y%m%d%H%M%S')
		filename = "savefile_%s_%04d" % (currtime, random.randint(0, 9999))

		# 保存路径
		path = os.path.join(self.dir, filename+ext)

		instance.save_changed = True

		return path


class School(models.Model):
	"""
	学校
	"""
	name = models.CharField(max_length=20, primary_key=True)


class QuestionRecordManager():

	def __init__(self):
		self.done_record = []
		self.wrong_record = []

	def refresh(self, done_record, wrong_record):

		self.done_record.clear()
		self.wrong_record = wrong_record

		for rec in done_record:
			self.done_record.append(rec['id'])

class Player(models.Model):
	"""
	人物
	"""
	name = models.CharField(max_length=20, primary_key=True)

	school = models.ForeignKey('School', null=False, on_delete=models.CASCADE)

	savefile = models.FileField(upload_to=SavefileUpload('player'), null=True, blank=True)
	
	def __init__(self, *args, **kwargs):

		super(Player, self).__init__(*args, **kwargs)
		self.save_changed = False
		self.save_data = None

		self.question_records = QuestionRecordManager()

	def getExactlyPath(self):

		base = settings.STATIC_URL
		path = os.path.join(base, str(self.savefile))

		if os.path.exists(path):
			return path
		else:
			raise ErrorException(ErrorType.FileNotFound)

	def refresh(self):
		getSavefileInfo(self)

	def getSavefileInfo(self):

		if self.save_changed:
			makeSavefileInfo(self)

		return self.save_data

	def makeSavefileInfo(self):
		salt = settings.SAVEFILE_SALT

		with open(self.getExactlyPath(), 'rb') as f:
			data = f.read()
			data = data[len(salt):]
			data = data.replace(salt,'')
			data = base64.b64decode(data)

		data = json.loads(data)

		player = self.getPlayerInfo(data)

		if 'name' in player and 'school' in player:
			if player['name'] == self.name and player['school'] == self.school.name:
				this.__refreshSaveData(data)
			else:
				raise ErrorException(ErrorType.PlayerDisMatch)
		else:
			raise ErrorException(ErrorType.SavefileError)

	def __getField(self, field, data=None):

		if data == None: 
			data = self.save_data

		if field in data:
			return data[field]
		raise ErrorException(ErrorType.SavefileError)

	def __refreshSaveData(self, data):

		self.save_data = data
		self.question_records.refresh(
			self.getQuestionRecordInfo(),
			self.getQuestionWrongInfo()
		)

	def getDoneRec(self):
		return self.question_records.done_record

	def getWrongRec(self):
		return self.question_records.wrong_record

	def getPlayerInfo(self, data=None):

		return self.__getField('player', data)

	def getSubjectValue(self, sid):

		return self.__getField('subjectParams', getPlayerInfo())[sid]

	def getRecordInfo(self, data=None):

		return self.__getField('record', data)

	def getQuestionRecordInfo(self, data=None):

		return self.__getField('questionRec', getRecordInfo(data))

	def getQuestionWrongInfo(self, data=None):

		return self.__getField('questionWrong', getRecordInfo(data))

	def getSaveTimeInfo(self, data=None):

		return datetime.strptime(self.__getField('curTime', data),"%d/%m/%Y %I:%M:%S %p")

