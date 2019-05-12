from django.db import models
from django.conf import settings
from utils.exception import ErrorType, ErrorException
import os, json, time, datetime, random

# Create your models here.


class School(models.Model):
	"""
	学校
	"""
	
	name = models.CharField(max_length=20, unique=False, primary_key=False)

	create_time = models.DateTimeField(auto_now_add=True)

	is_deleted = models.BooleanField(default=False)

	def __str__(self):
		return self.name

class QuestionRecordManager:

	def __init__(self):
		self.done_record = []
		self.wrong_record = []

	def refresh(self, done_record, wrong_record):

		self.done_record.clear()
		self.wrong_record.clear()

		for rec in wrong_record['list']:
			self.wrong_record.append(int(rec))

		for rec in done_record['list']:
			self.done_record.append(int(rec['id']))

class Player(models.Model):
	"""
	人物
	"""

	name = models.CharField(max_length=20, unique=False, primary_key=False)

	school = models.ForeignKey('School', null=False, on_delete=models.CASCADE)

	#savefile = models.FileField(upload_to=SavefileUpload('player'), null=True, blank=True)
	
	save_data_text = models.TextField(null=True, blank=True)

	create_time = models.DateTimeField(auto_now_add=True)

	is_deleted = models.BooleanField(default=False)

	def __init__(self, *args, **kwargs):

		super(Player, self).__init__(*args, **kwargs)
		#self.save_changed = False
		print ("init")
		
		self.save_data = None

		self.question_records = QuestionRecordManager()

		self.loadSaveData()

		print ("save_data = "+str(self.save_data))

	def __str__(self):
		return self.name

	"""
	def getExactlyPath(self):

		base = settings.STATIC_URL
		path = os.path.join(base, str(self.savefile))

		print(path)

		if os.path.exists(path):
			return path
		else:
			raise ErrorException(ErrorType.FileNotFound)

	
	def refresh(self):

		self.makeSavefileInfo()

	def getSavefileInfo(self):

		if self.save_changed:
			self.makeSavefileInfo()

		return self.save_data

	def makeSavefileInfo(self):
		salt = settings.SAVEFILE_SALT

		with open(self.getExactlyPath(), 'rb') as f:
			data = f.read().decode()
			data = data[len(salt):]
			data = data.replace(salt,'')
			data = base64.b64decode(data)

		self.save_data_text = data

	"""

	def checkSaveData(self, data):

		player = self.getPlayerInfo(data)

		name = self.__getField('name', player)
		school = self.__getField('school', player)

		print (name)
		print (school)

		print (self.name)
		print (self.school.name)

		if name != self.name or school != self.school.name:
			raise ErrorException(ErrorType.PlayerDisMatch)
		
		return True

	# 加载存档数据
	def loadSaveData(self):

		if self.save_data_text == None:
			return

		try:
			data = json.loads(self.save_data_text)
		except:
			raise ErrorException(ErrorType.SavefileError)

		print ("loadSaveData: "+str(data))

		if self.checkSaveData(data):
			self.__refreshSaveData(data)

	def __getField(self, field, data=None):

		if data == None: 
			if self.save_data != None: 
				data = self.save_data
			else:
				raise ErrorException(ErrorType.NoSavefileInfo)

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

		player_info = self.getPlayerInfo()
		subject_sel = self.__getField('subjectSel', player_info)

		if subject_sel==0: delta = 4 # 文科
		if subject_sel==1: delta = 1 # 理科

		return self.__getField('subjectParams', player_info)['list'][sid-delta]

	def getRecordInfo(self, data=None):

		return self.__getField('record', data)

	def getQuestionRecordInfo(self, data=None):

		return self.__getField('questionRec', self.getRecordInfo(data))

	def getQuestionWrongInfo(self, data=None):

		return self.__getField('questionWrong', self.getRecordInfo(data))

	def getSaveTimeInfo(self, data=None):

		return datetime.strptime(self.__getField('curTime', data),
			"%d/%m/%Y %H:%M:%S")

	def getSaveTimeInfoForAdmin(self):
		if self.save_data_text == None:
			return '无存档信息'
		else:
			return self.__getField('curTime', None)

