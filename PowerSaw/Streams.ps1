$counter = 0
while ($true)
{
   switch (Get-Random -Minimum 1 -Maximum 7)
   {
      1
      {
         Write-Error 'Oops!'
      }
      2
      {
         Write-Warning 'Help!'
      }
      3
      {
         Write-Verbose 'Oh no!'
      }
      4
      {
         Write-Information 'Hehe!'
      }
      5
      {
         Write-Debug 'Grok!'
      }
      default
      {
         $_
         $counter++
      }
   }
   Start-Sleep -Seconds 1
   if ($counter -ge 6)
   {
      exit
   }
}
