from django.contrib import admin
from .models import Channel, News


class ChannelModelAdmin(admin.ModelAdmin):
    list_display = ('title', 'lastid', 'lasttime')


class NewsModelAdmin(admin.ModelAdmin):
    list_display = ('title',)


admin.site.register(News, NewsModelAdmin)
admin.site.register(Channel, ChannelModelAdmin)