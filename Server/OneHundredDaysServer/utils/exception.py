from enum import Enum

class ErrorType(Enum):
	# Common
	Success				= 0  # 成功，无错误
	InvalidRequest		= 1  # 非法的请求方法
	ParameterError		= 2  # 参数错误

	# Savefile
	FileNotFound		= 10 # 文件未找到
	PlayerDisMatch		= 11 # 玩家不匹配
	SavefileError		= 12 # 存档错误
	NoSavefileInfo		= 13 # 无存档信息

	# Player
	PlayerNotExist		= 20 # 玩家不存在
	PlayerExist			= 21 # 玩家已存在
	SchoolNotExist		= 22 # 学校不存在

	# Question
	QuestionNoAnswer	= 30 # 题目无答案

class ErrorException(Exception):

	error_dict = {
		# Common
		ErrorType.Success:			"",
		ErrorType.InvalidRequest:	"非法的请求方法！",
		ErrorType.ParameterError:	"参数错误！",

		# Savefile
		ErrorType.FileNotFound:		"未找到存档文件！",
		ErrorType.PlayerDisMatch:	"玩家不匹配！",
		ErrorType.SavefileError:	"存档错误！",
		ErrorType.NoSavefileInfo:	"无存档信息！",

		# Player
		ErrorType.PlayerNotExist:	"玩家不存在！",
		ErrorType.PlayerExist:		"玩家已存在！",
		ErrorType.SchoolNotExist:	"学校不存在！",

		# Question
		ErrorType.QuestionNoAnswer:	"题目无答案！",
	}

	def __init__(self, error_type: ErrorType):
		self.error_type = error_type
		self.msg = ErrorException.error_dict[error_type]

	def __str__(self):
		return self.msg