
from django.contrib import admin

from player_module.models import Player, School
# Register your models here.
class SchoolAdmin(admin.ModelAdmin):

    list_display = ['id','name']

class PlayerAdmin(admin.ModelAdmin):

    list_display = ['id','name','school','savefile']


admin.site.register(Player, PlayerAdmin)

admin.site.register(School, SchoolAdmin)