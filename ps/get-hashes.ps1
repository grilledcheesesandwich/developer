param(
    [string] $file = $(throw 'a filename is required'), 
    [string[]] $algorithms = ('md5','SHA1','sha256')
)

foreach ($algorithm in $algorithms)
{
    "$algorithm hash for $file = $(hash $file $algorithm)\n"
} 