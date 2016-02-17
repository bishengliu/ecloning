from django.core.management.base import AppCommand, CommandError
from django.core.management.sql import sql_reset
from django.core.management.color import no_style
from django.db import connections

class Command(AppCommand):
    help = "**********\nThis command resets data for any django app, the difference with the built-in command\n\n    '$ python manage.py reset <app_name>'\n\nis that when a sql statement fails, it jumps to the next statement generated by command\n\n    '$ python manage.py sqlreset <app_name>'\n\nUseful when the original reset fail when droping CONSTRAINTS\n**********"
    output_transaction = True
    def handle_app(self, app, **options):
        connection = connections['default']
        self.style = no_style()
        custom_reset_statements = sql_reset(app, self.style, connection)
        cursor = connection.cursor()
        def execute_sqlreset():
            failed_statements = []
            for sql in custom_reset_statements:
                print 'statement>>>> ' + sql
                try:
                    cursor.execute(sql)
                except Exception,e:
                    if e[0] == 1025:
                        failed_statements.append(sql)

            if failed_statements:
                print "These statements failed: "
                for s in failed_statements:
                    print s
        execute_sqlreset()
