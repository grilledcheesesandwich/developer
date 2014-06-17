param( $find, $replace, $includes )

# for every file that contains a match
select-string $find -list -path $includes  | % { 

    # read its contents into an array of strings
    (get-content $_.Path)  | % {

         # replace everything as necessary
         $_ -replace $find, $replace 

     # write it back out
     } | set-content $_.Path 
}
